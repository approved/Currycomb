using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
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
                    Log.Information("Read wrapped packet: {@wpkt}");

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

            // TODO: Implement config file
            CancellationToken ct = new();

            ClientWebSocket eventSocket = new();

            Uri webSocketUri = new("ws://127.0.0.1:10002/");
            await eventSocket.ConnectAsync(webSocketUri, ct);
            Log.Information("Connecting to BroadcastService @ {@wsUri}", webSocketUri);

            TcpListener listener = new(IPAddress.Any, 10001);
            listener.Start();
            Log.Information("Starting listener on {@listener}", listener.LocalEndpoint.ToString());

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
