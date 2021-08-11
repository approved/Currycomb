using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network;
using System.IO;
using System.Threading.Channels;
using Currycomb.Common.Network.Broadcast;
using Newtonsoft.Json;
using System.Text;

namespace Currycomb.PlayService
{
    public class Program
    {
        private static readonly string LogFileName = $"logs/play_service/play_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";

        public static async Task HandleWrappedPacketStream(ChannelWriter<IGameEvent> evt, ClientWebSocket eventSocket, WrappedPacketStream wps)
        {
            PlayPacketHandler pph = new();
            GamePacketRouter<Context> router = pph.Router;

            try
            {
                while (true)
                {
                    WrappedPacketContainer wpkt = await wps.ReadAsync();
                    Log.Information($"Read wrapped packet: {wpkt}");

                    WrappedPacket wrapped = wpkt.Packet;

                    using MemoryStream memoryStream = new(wrapped.GetOrCreatePacketByteArray(), false);

                    Context context = new(evt, wrapped.ClientId, wps, eventSocket);
                    await router.HandlePacketAsync(context, memoryStream, (uint)wrapped.Data.Length);
                    await wps.SendAckAsync(wpkt);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception in wrapped packet stream");
            }
        }

        public static async Task HandleEventStream(ChannelWriter<IGameEvent> evt, ClientWebSocket eventSocket)
        {
            try
            {
                byte[] buffer = new byte[1024];
                CancellationToken ct = default;

                while (true)
                {
                    Log.Information("Waiting for event...");
                    WebSocketReceiveResult data = await eventSocket.ReceiveAsync(buffer, ct);
                    switch (data.MessageType)
                    {
                        case WebSocketMessageType.Text:
                            ComEvent comEvent = JsonConvert.DeserializeObject<ComEvent>(Encoding.UTF8.GetString(buffer, 0, data.Count)) ?? throw new Exception("Invalid event");
                            Log.Information("Received event: {@comEvent}", comEvent);

                            switch (comEvent.Subject)
                            {
                                case "client::changed_state":
                                    var payload = JsonConvert.DeserializeObject<PayloadStateChange>(comEvent.Payload) ?? throw new Exception("Invalid payload");

                                    if (payload.State == State.Play)
                                    {
                                        await evt.WriteAsync(new EvtPlayerConnected(payload.Client));
                                    }

                                    break;
                            }

                            break;
                        default:
                            Log.Error("Received unknown message type: {@msgType}", data.MessageType);
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception in event stream");
            }
        }

        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Async(x => x.Console())
                .WriteTo.Async(x => x.File(LogFileName))
                .CreateLogger();

            CancellationTokenSource cts = new();
            GameInstance game = new();
            Thread gameThread = new(async () => await game.Run(cts.Token));

            gameThread.Start();

            // TODO: Implement config file
            ClientWebSocket eventSocket = new();

            Uri webSocketUri = new("ws://127.0.0.1:10002/");
            await eventSocket.ConnectAsync(webSocketUri, new());
            Log.Information("Connecting to BroadcastService @ {@wsUri}", webSocketUri);

            var eventSocketTask = HandleEventStream(game.EventWriter, eventSocket);

            while (true)
            {
                Log.Information("Connecting");

                using TcpClient client = new("localhost", 10003);
                Log.Information("Connected to {{ {@remoteEndpoint} }}", client.Client.RemoteEndPoint!.ToString());

                WrappedPacketStream wps = new(client.GetStream());
                CancellationTokenSource wpsCts = new();
                Task wpsTask = wps.RunAsync(wpsCts.Token);

                Log.Information("Starting wrapped packet stream");

                await await Task.WhenAny(
                    HandleWrappedPacketStream(game.EventWriter, eventSocket, wps),
                    eventSocketTask
                );

                wpsCts.Cancel();
            }
        }
    }
}
