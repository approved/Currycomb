using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using Currycomb.Common.Network.Minecraft;
using Currycomb.Common.Network;
using System.IO;

namespace Currycomb.PlayService
{
    public class Program
    {
        private static readonly string LogFileName = $"logs/play_service/play_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";

        public static async Task HandleWrappedPacketStream(ClientWebSocket eventSocket, WrappedPacketStream wps)
        {
            PlayPacketHandler pph = new();
            PacketRouter<Context> router = pph.Router;

            try
            {
                while (true)
                {
                    WrappedPacketContainer wpkt = await wps.ReadAsync();
                    Log.Information($"Read wrapped packet: {wpkt}");

                    WrappedPacket wrapped = wpkt.Packet;

                    using MemoryStream memoryStream = new(wrapped.GetOrCreatePacketByteArray(), false);

                    Context context = new(wrapped.ClientId, wps, eventSocket);
                    await router.HandlePacketAsync(context, memoryStream, (uint)wrapped.Data.Length);
                    await wps.SendAckAsync(wpkt);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception in wrapped packet stream");
            }
        }
        public static async Task Main()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Async(x => x.Console())
                .WriteTo.Async(x => x.File(LogFileName))
                .CreateLogger();

            // TODO: Implement config file
            CancellationToken ct = new();

            ClientWebSocket eventSocket = new();

            Log.Information("Connecting to BroadcastService");
            await eventSocket.ConnectAsync(new("ws://127.0.0.1:10002/"), ct);

            Log.Information("Connected, starting listener");
            TcpListener listener = new(IPAddress.Any, 10003);
            listener.Start();

            while (true)
            {
                Log.Information("Awaiting connection");

                using TcpClient client = await listener.AcceptTcpClientAsync();
                Log.Information("Received client");

                WrappedPacketStream wps = new(client.GetStream());
                CancellationTokenSource wpsCts = new();
                Task wpsTask = wps.RunAsync(wpsCts.Token);

                await HandleWrappedPacketStream(eventSocket, wps);
                wpsCts.Cancel();
            }
        }
    }
}

