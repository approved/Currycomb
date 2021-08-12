using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Currycomb.AuthService.Configuration;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IO;
using Serilog;

namespace Currycomb.AuthService
{
    public class Program
    {
        private static readonly string LogFileName = $"logs/auth_service/auth_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";
        private static readonly RecyclableMemoryStreamManager MsManager = new RecyclableMemoryStreamManager();

        public static async Task HandleWrappedPacketStream(ClientWebSocket eventSocket, WrappedPacketStream wps)
        {
            ConcurrentDictionary<Guid, State> ClientState = new();

            RSA rsa = RSA.Create(1024);

            AuthPacketHandler aph = new();
            GamePacketRouter<Context> gameRouter = aph.Router;

            try
            {
                while (true)
                {
                    WrappedPacketContainer wpkt = await wps.ReadAsync();
                    Log.Information("Read wrapped packet: {wpkt}", wpkt);

                    WrappedPacket wrapped = wpkt.Packet;

                    using MemoryStream memoryStream = new(wrapped.GetOrCreatePacketByteArray(), false);

                    Context context = new(wrapped.ClientId, rsa, ClientState, wps, eventSocket);
                    await gameRouter.HandlePacketAsync(context, memoryStream, (uint)wrapped.Data.Length);
                    await wps.SendAckAsync(wpkt);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception in wrapped packet stream");
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
                .AddEnvironmentVariables($"CC_CFG_{env}_")
                .AddCommandLine(args)
                .Build();

            var appConfig = configuration.Get<AppConfiguration>();
            Log.Information("Loaded configuration: {@appConfig}", appConfig);

            // TODO: Implement config file
            CancellationToken ct = new();
            ClientWebSocket eventSocket = new();

            Uri broadcastUri = appConfig.Broadcast.Uri;
            Uri gatewayUri = appConfig.Gateway.Uri;

            await eventSocket.ConnectAsync(broadcastUri, ct);
            Log.Information("Connecting to BroadcastService @ {@wsUri}", broadcastUri);

            while (true)
            {
                Log.Information("Connecting to Gateway @ {@gatewayUri}", gatewayUri);
                using TcpClient client = new TcpClient(gatewayUri.Host, gatewayUri.Port);
                Log.Information("Connected");

                WrappedPacketStream wps = new(client.GetStream(), MsManager);
                CancellationTokenSource wpsCts = new();
                Task wpsTask = wps.RunAsync(wpsCts.Token);

                await HandleWrappedPacketStream(eventSocket, wps);
                wpsCts.Cancel();
            }
        }
    }
}
