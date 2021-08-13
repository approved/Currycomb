using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Currycomb.Common.Configuration;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using Microsoft.Extensions.Configuration;
using Microsoft.IO;
using Serilog;

namespace Currycomb.AuthService
{
    public class Program
    {
        private static readonly string LogFileName = $"logs/auth_service/auth_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";
        private static readonly RecyclableMemoryStreamManager MsManager = new RecyclableMemoryStreamManager();

        public static async Task HandleWrappedPacketStream(WrappedPacketStream wps, CancellationToken ct = default)
        {
            ConcurrentDictionary<Guid, State> ClientState = new();

            RSA rsa = RSA.Create(1024);

            GamePacketRouter<Context> gameRouter = new AuthPacketHandler().Router;
            MetaPacketRouter<Context> metaRouter = new MetaPacketHandler().Router;

            while (!ct.IsCancellationRequested)
            {
                WrappedPacketContainer wpkt = await wps.ReadAsync(false, ct);
                Log.Information("Read wrapped packet: {wpkt}", wpkt);

                WrappedPacket wrapped = wpkt.Packet;
                Context context = new(wrapped.ClientId, rsa, ClientState, wps);

                if (wpkt.IsMetaPacket)
                {
                    using MemoryStream memoryStream = new(wrapped.GetOrCreatePacketByteArray(), false);
                    using BinaryReader binaryReader = new(memoryStream);

                    await metaRouter.HandlePacketAsync(context, binaryReader);
                }
                else
                {
                    using MemoryStream memoryStream = new(wrapped.GetOrCreatePacketByteArray(), false);

                    await gameRouter.HandlePacketAsync(context, memoryStream, (uint)wrapped.Data.Length);
                }

                await wps.SendAckAsync(wpkt);
            }
        }

        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Async(x => x.Console())
                .WriteTo.Async(x => x.File(LogFileName))
                .CreateLogger();

            Log.Information("AuthService starting");
            var env = Environment.GetEnvironmentVariable("CC_ENV") ?? "live";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                // Base config
                .AddJsonFile($"cfg.json", true, true)
                .AddJsonFile($"cfg.{env}.json", true, true)
                // Auth-specific config
                .AddJsonFile($"cfg.svc_auth.json", true, true)
                .AddJsonFile($"cfg.svc_auth.{env}.json", true, true)
                // Env vars and command line args override JSON configs
                .AddEnvironmentVariables($"CC_CFG_")
                .AddCommandLine(args)
                .Build();

            Log.Information("Loaded configuration: {@appConfig}", configuration.GetDebugView());

            GatewayConfiguration gateway = configuration
                .GetSection(nameof(GatewayConfiguration))
                .Get<GatewayConfiguration>();

            if (gateway == null)
            {
                Log.Fatal("No gateway configuration found.");
                Log.CloseAndFlush();
                return;
            }

            while (true)
            {
                CancellationTokenSource cts = new();

                try
                {
                    Log.Information("Attempting to connect to Gateway @ {gatewayHost}:{gatewayPort}", gateway.Host, gateway.Port);
                    using TcpClient client = new TcpClient(gateway.Host, gateway.Port);
                    Log.Information("Connected");

                    WrappedPacketStream wps = new(client.GetStream(), MsManager);

                    await await Task.WhenAny(
                        wps.RunAsync(cts.Token),
                        HandleWrappedPacketStream(wps, cts.Token)
                    );
                }
                // No connection could be made because the target machine actively refused it.
                catch (SocketException e) when (e.ErrorCode == 10061) { }
                // An existing connection was forcibly closed by the remote host.
                catch (IOException e) when (e.InnerException is SocketException se && se.ErrorCode == 10054) { }
                // We cancelled things.
                catch (OperationCanceledException) { }
                // Unknown exception.
                catch (Exception e)
                {
                    Log.Error(e, "Hit an unknown exception, reconnecting in {delay}.", gateway.ReconnectDelay);
                }

                cts.Cancel();

                await Task.Delay(gateway.ReconnectDelay);
            }
        }
    }
}
