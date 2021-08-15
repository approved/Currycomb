using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Currycomb.Common;
using Currycomb.Common.Extensions;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;
using Serilog;

namespace Currycomb.Gateway
{
    public class ServiceListener
    {
        private readonly IPEndPoint _endpoint;
        private readonly ServiceCollection _services;

        public ServiceListener(IPEndPoint endpoint, ServiceCollection services)
        {
            _endpoint = endpoint;
            _services = services;
        }

        public async Task AcceptConnections(CancellationToken ct = default)
        {
            Log.Information("Starting listener on port {port}", _endpoint.Port);
            TcpListener listener = new TcpListener(_endpoint);
            listener.Start();

            while (!ct.IsCancellationRequested)
            {
                TcpClient client = await listener.AcceptTcpClientAsync(ct);
                if (ct.IsCancellationRequested)
                    return;

                // TODO: Accepting services should be handled async and one connection that has yet to announce itself
                // should not block another service that wants to connect.

                try
                {
                    WrappedPacketStream newWps = new(client.GetStream());
                    Log.Information("Accepted service connection from {ip}", client.Client.RemoteEndPoint);

                    WrappedPacketContainer wpc = await newWps.RunOnceAsync(true, ct);
                    if (!wpc.IsMetaPacket)
                    {
                        Log.Error("Received non-meta as first packet from {ip}", client.Client.RemoteEndPoint);
                        continue;
                    }

                    WrappedPacket wp = wpc.Packet;
                    if (wp.ClientId != Constants.ClientId.ToGatewayGuid)
                    {
                        Log.Error("Received packet not targeting the gateway (client was {clientId}) as first packet from {ip}.", wp.ClientId, client.Client.RemoteEndPoint);
                        continue;
                    }

                    using MemoryStream ms = new(wp.GetOrCreatePacketByteArray());
                    using BinaryReader br = new(ms);

                    MetaPacketHeader header = MetaPacketHeader.Read(br);
                    if (header.PacketId != MetaPacketId.Announce)
                    {
                        Log.Error("Received non-announce packet ({pktId}) as first packet from {ip}", header.PacketId, client.Client.RemoteEndPoint);
                        continue;
                    }

                    Log.Information("Received announce packet from {ip}", client.Client.RemoteEndPoint);
                    await newWps.SendAckAsync(wpc);

                    Log.Information("Sent ack to {ip}", client.Client.RemoteEndPoint);
                    PacketAnnounce announce = PacketAnnounce.Read(br);
                    Log.Information("Adding service instance {@announce}", announce);
                    await _services.Add(new(announce, newWps));
                }
                catch (Exception e)
                {
                    Log.Error(e, "Failed to read meta packet from {ip}", client.Client.RemoteEndPoint);
                    continue;
                }
            }
        }
    }
}
