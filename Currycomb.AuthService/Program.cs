using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Minecraft;
using Currycomb.Common.Network.Minecraft.Packets;
using System.Collections.Generic;

namespace Currycomb.AuthService
{
    public class Program
    {
        private static readonly string LogFileName = $"logs/auth_service/auth_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";

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
            await eventSocket.ConnectAsync(new Uri("ws://127.0.0.1:10002/"), ct);

            Log.Information("Connected, starting listener");
            TcpListener listener = new(IPAddress.Any, 10001);
            listener.Start();

            while (true)
            {
                Log.Information("Awaiting connection");

                using TcpClient client = await listener.AcceptTcpClientAsync();
                Log.Information("Received client");

                WrappedPacketStream wps = new WrappedPacketStream(client.GetStream());
                CancellationTokenSource wpsCts = new CancellationTokenSource();
                Task wpsTask = wps.RunAsync(wpsCts.Token);

                await HandleWrappedPacketStream(eventSocket, wps);
                wpsCts.Cancel();
            }
        }

        public static async Task HandleWrappedPacketStream(ClientWebSocket eventSocket, WrappedPacketStream wps)
        {
            // If packets are ever processed in parallel, this needs to be swapped to a ConcurrentDictionary or locked on
            Dictionary<Guid, State> ClientState = new();

            async Task PacketHandshake(Guid id, PacketHandshake pkt)
            {
                ClientState.Add(id, pkt.State);
                ComEvent ev = ComEvent.Create(EventType.ChangedState, new PayloadStateChange(id, pkt.State));
                await eventSocket.SendAsync(Encoding.UTF8.GetBytes(ev.Serialize()), WebSocketMessageType.Text, true, default);
            }

            async Task PacketLoginStart(Guid id, PacketLoginStart pkt)
            {
                ComEvent ev = ComEvent.Create(EventType.ChangedState, new PayloadStateChange(id, State.Play));
                await eventSocket.SendAsync(Encoding.UTF8.GetBytes(ev.Serialize()), WebSocketMessageType.Text, true, default);

                PacketLoginSuccess pktRes = new(Guid.NewGuid(), "Fiskpinne");
                await wps.SendAsync(new WrappedPacket(id, await pktRes.ToBytes(PacketId.LoginSuccess)));
                Log.Information("Replied to PacketLoginStart");
            }

            async Task PacketRequest(Guid id, PacketRequest pkt)
            {
                PacketResponse pktRes = new("{\"version\":{\"name\": \"1.17.1\",\"protocol\": 756},\"players\":{\"max\":100,\"online\":5},\"description\":{\"text\":\"Hello world!\"}}");
                await wps.SendAsync(new WrappedPacket(id, await pktRes.ToBytes(PacketId.Response)));
                Log.Information("Replied to PacketRequest");
            }

            async Task PacketPing(Guid id, PacketPing pkt)
            {
                await wps.SendAsync(new WrappedPacket(id, await new PacketPong(pkt.Timestamp).ToBytes(PacketId.Pong)));
                Log.Information("Replied to PacketPing");
            }

            var router = PacketRouter.New()
                .On<PacketHandshake>(PacketHandshake)
                .On<PacketLoginStart>(PacketLoginStart)
                .On<PacketRequest>(PacketRequest)
                .On<PacketPing>(PacketPing)
                .Build();

            try
            {
                while (true)
                {
                    WrappedPacketContainer wpkt = await wps.ReadAsync();
                    Log.Information($"Read wrapped packet: {wpkt}");

                    WrappedPacket wrapped = wpkt.Packet;
                    State state = ClientState.TryGetValue(wrapped.ClientId, out var x) ? x : State.Handshake;

                    await router.HandlePacketAsync(state, wrapped);
                    await wps.SendAckAsync(wpkt);
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Exception in wrapped packet stream");
            }
        }
    }
}
