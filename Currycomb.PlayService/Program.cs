using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network;
using System.IO;
using System.Threading.Channels;
using Currycomb.Common.Network.Meta;
using Microsoft.IO;
using Currycomb.Common.Configuration;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace Currycomb.PlayService
{
    public class Program
    {
        private static readonly string LogFileName = $"logs/play_service/play_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";
        private static readonly RecyclableMemoryStreamManager MsManager = new RecyclableMemoryStreamManager();

        public static async Task HandleWrappedPacketStreamOutgoing(ChannelReader<WrappedPacket> outgoing, WrappedPacketStream wps)
        {
            while (true)
            {
                var packet = await outgoing.ReadAsync();
                await wps.SendAsync(false, packet);
            }
        }

        public static async Task HandleWrappedPacketStreamIncoming(
            GamePacketRouter<Context> gameRouter,
            MetaPacketRouter<Context> metaRouter,
            ChannelWriter<IGameEvent> evt,
            WrappedPacketStream wps)
        {
            while (true)
            {
                Log.Information($"Waiting for wrapped packet.");
                WrappedPacketContainer wpkt = await wps.ReadAsync();
                Log.Information("Read wrapped packet: {wpkt}", wpkt);
                Log.Information("Read wrapped packet: {@data}", wpkt.Packet.Data.ToArray());

                WrappedPacket wrapped = wpkt.Packet;
                Context context = new(evt, wrapped.ClientId, wps);

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

            Log.Information("PlayService starting");
            var env = Environment.GetEnvironmentVariable("CC_ENV") ?? "live";
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                // Base config
                .AddJsonFile($"cfg.json", true, true)
                .AddJsonFile($"cfg.{env}.json", true, true)
                // Play-specific config
                .AddJsonFile($"cfg.svc_play.json", true, true)
                .AddJsonFile($"cfg.svc_play.{env}.json", true, true)
                // Env vars and command line args override JSON configs
                .AddEnvironmentVariables($"CC_CFG_")
                .AddCommandLine(args)
                .Build();

            Log.Information("Loaded configuration: {@configuration}", configuration.GetDebugView());

            GatewayConfiguration gateway = configuration
                .GetSection(nameof(GatewayConfiguration))
                .Get<GatewayConfiguration>();

            if (gateway == null)
            {
                Log.Fatal("No gateway configuration found.");
                Log.CloseAndFlush();
                return;
            }

            Log.Information("Loaded configuration: {@gateway}", gateway);

            Channel<WrappedPacket> outgoing = Channel.CreateUnbounded<WrappedPacket>();

            CancellationTokenSource cts = new();
            GameInstance game = new(MsManager, outgoing.Writer);
            Thread gameThread = new(async () => await game.Run(cts.Token));

            gameThread.Start();

            GamePacketRouter<Context> gameRouter = new PlayPacketHandler().Router;
            MetaPacketRouter<Context> metaRouter = new MetaPacketHandler().Router;

            while (true)
            {
                CancellationTokenSource sessionCts = new();

                try
                {
                    Log.Information("Attempting to connect to Gateway @ {gatewayHost}:{gatewayPort}", gateway.Host, gateway.Port);
                    using TcpClient client = new TcpClient(gateway.Host, gateway.Port);
                    Log.Information("Connected");

                    WrappedPacketStream wps = new(client.GetStream(), MsManager);

                    // Needs to run for us to receive ack packets
                    Task wpsLoop = wps.RunAsync(cts.Token);

                    Log.Information("Sending Announce and waiting for ack.");
                    await wps.Announce(new(
                        "PlayService",
                        PlayService.Constants.ServiceId,
                        metaRouter.Packets.ToArray(),
                        gameRouter.Packets.ToArray()));

                    Log.Information("Received ack, continuing.");

                    await await Task.WhenAny(
                        wpsLoop,
                        HandleWrappedPacketStreamOutgoing(outgoing.Reader, wps),
                        HandleWrappedPacketStreamIncoming(gameRouter, metaRouter, game.EventWriter, wps));
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

                sessionCts.Cancel();

                await Task.Delay(gateway.ReconnectDelay);
            }
        }
    }
}
