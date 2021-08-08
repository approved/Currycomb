﻿using System;
using System.Collections.Concurrent;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using Currycomb.Common.Extensions;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Minecraft;

namespace Currycomb.AuthService
{

    public class Program
    {
        static ConcurrentDictionary<Guid, State> ClientState = new();
        static ClientWebSocket clientWebSocket = new();

        private static readonly string LogFileName = $"logs/auth_service/auth_{DateTime.Now:yyyy-MM-dd-HH-mm-ss}_{Environment.ProcessId}.txt";

        public static async Task HandleWrappedPacketStream(WrappedPacketStream wps)
        {
            // TODO: should break on connection lost
            while (true)
            {
                Log.Information("Reading wrapped packet");
                WrappedPacketContainer wpkt = await wps.ReadAsync();
                Log.Information($"Read wrapped packet: {wpkt}");

                {
                    WrappedPacket wrapped = wpkt.Packet;
                    State state = ClientState.TryGetValue(wrapped.ClientId, out var x) ? x : State.Handshake;

                    // TODO: Avoid copying here if/when possible

                    using MemoryStream memoryStream = new(wrapped.Packet.ToArray());
                    PacketHeader header = await PacketHeader.ReadAsync(memoryStream, (uint)wrapped.Packet.Length);
                    Log.Information($"Read packet header: {header}");

                    PacketId id = PacketIdExt.FromRaw(BoundTo.Server, state, header.PacketId);
                    Log.Information($"Read packet id: {id}");

                    IPacket packet = await PacketReader.ReadAsync(id, memoryStream);
                    Log.Information($"Read packet: {packet}");

                    switch (packet)
                    {
                        case PacketHandshake pkt:
                            ClientState.AddOrUpdate(wrapped.ClientId, pkt.State, (_, _) => pkt.State);
                            ComEvent ev = ComEvent.Create(EventType.ChangedState, new PayloadStateChange(wrapped.ClientId, pkt.State));

                            await wps.SendAckAsync(wpkt);
                            await clientWebSocket.SendAsync(Encoding.UTF8.GetBytes(ev.Serialize()), WebSocketMessageType.Text, true, default);

                            break;
                        case PacketLoginStart pkt:
                            {
                                await wps.SendAckAsync(wpkt);

                                PacketLoginSuccess pktRes = new(Guid.NewGuid(), "Fiskpinne");

                                // TODO: Very inefficient, avoid triple clone for the data

                                using MemoryStream msPacket = new();
                                await pktRes.WriteAsync(msPacket);

                                using MemoryStream msFull = new();
                                await msFull.Write7BitEncodedIntAsync((int)msPacket.Length);
                                await msPacket.CopyToAsync(msFull);

                                await wps.SendAsync(new WrappedPacket(wrapped.ClientId, msFull.ToArray()));
                                Log.Information("Replied to PLS");

                                break;
                            }
                        case PacketRequest pkt:
                            {

                                await wps.SendAckAsync(wpkt);

                                PacketResponse pktRes = new("{\r\n    \"version\": {\r\n        \"name\": \"1.8.7\",\r\n        \"protocol\": 47\r\n    },\r\n    \"players\": {\r\n        \"max\": 100,\r\n        \"online\": 5,\r\n        \"sample\": [\r\n            {\r\n                \"name\": \"thinkofdeath\",\r\n                \"id\": \"4566e69f-c907-48ee-8d71-d7ba5aa00d20\"\r\n            }\r\n        ]\r\n    },\r\n    \"description\": {\r\n        \"text\": \"Hello world\"\r\n    }\r\n}");

                                // TODO: Very inefficient, avoid triple clone for the data
                                using MemoryStream msPacket = new();
                                await pktRes.WriteAsync(msPacket);

                                using MemoryStream msFull = new();
                                await msFull.Write7BitEncodedIntAsync((int)msPacket.Length);
                                await msPacket.CopyToAsync(msFull);

                                await wps.SendAsync(new WrappedPacket(wrapped.ClientId, msFull.ToArray()));
                                Log.Information("Replied to PLS");

                                break;
                            }
                        default:
                            await wps.SendAckAsync(wpkt);
                            Log.Information($"Invalid packet for state: {state}, {header.PacketId}");
                            // throw new FormatException($"Invalid packet for state: {state}, {header.PacketId}");
                            break;
                    }
                }
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

            Log.Information("Connecting to BroadcastService");
            await clientWebSocket.ConnectAsync(new Uri("ws://127.0.0.1:10002/"), ct);

            Log.Information("Connected, starting listener");
            TcpListener listener = new(IPAddress.Any, 10001);
            listener.Start();

            Task tcpClientTask = Task.Run(async () =>
            {
                while (true)
                {
                    Log.Information("Awaiting connection");

                    using TcpClient client = await listener.AcceptTcpClientAsync();
                    Log.Information("Received client");

                    WrappedPacketStream wps = new WrappedPacketStream(client.GetStream());
                    CancellationTokenSource wpsCts = new CancellationTokenSource();
                    Task wpsTask = wps.RunAsync(wpsCts.Token);

                    try
                    {
                        await HandleWrappedPacketStream(wps);
                    }
                    catch (Exception e)
                    {
                        Log.Information($"Exception in wrapped packet stream: {e}");
                    }

                    wpsCts.Cancel();
                }
            });

            Task webSocketTask = Task.Run(async () =>
            {
                while (true)
                {
                    byte[] b = new byte[1024];
                    WebSocketReceiveResult result = await clientWebSocket.ReceiveAsync(b, ct);
                }
            });

            Log.Information("Initialized.");
            await Task.WhenAll(tcpClientTask, webSocketTask);
        }
    }

    class AuthService
    {

    }
}
