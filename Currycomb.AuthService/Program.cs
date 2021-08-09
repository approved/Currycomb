using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Minecraft;
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

            AuthPacketHandler aph = new();
            PacketRouter<Context> router = aph.Router;

            try
            {
                while (true)
                {
                    WrappedPacketContainer wpkt = await wps.ReadAsync();
                    Log.Information($"Read wrapped packet: {wpkt}");

                    WrappedPacket wrapped = wpkt.Packet;

                    using MemoryStream memoryStream = new(wrapped.GetOrCreatePacketByteArray(), false);

                    Context context = new(wrapped.ClientId, ClientState, wps, eventSocket);
                    await router.HandlePacketAsync(context, memoryStream, (uint)wrapped.Data.Length);
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

            // TODO: Implement config file
            CancellationToken ct = new();

            ClientWebSocket eventSocket = new();

            Log.Information("Connecting to BroadcastService");
            await eventSocket.ConnectAsync(new("ws://127.0.0.1:10002/"), ct);

            Log.Information("Connected, starting listener");
            TcpListener listener = new(IPAddress.Any, 10001);
            listener.Start();

            while (true)
            {
                Log.Information("Awaiting connection");

                using TcpClient client = await listener.AcceptTcpClientAsync();
                Log.Information("Received client");

                WrappedPacketStream wps = new(client.GetStream(), MsManager);
                CancellationTokenSource wpsCts = new();
                Task wpsTask = wps.RunAsync(wpsCts.Token);

                await HandleWrappedPacketStream(eventSocket, wps);
                wpsCts.Cancel();
            }
        }
    }
}
