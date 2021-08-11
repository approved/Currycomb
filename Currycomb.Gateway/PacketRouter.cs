using System.Threading.Tasks;
using Serilog;
using Currycomb.Gateway.Network;
using Currycomb.Common.Network;
using System.Threading;
using Currycomb.Gateway.ClientData;

namespace Currycomb.Gateway
{
    public class PacketRouter
    {
        private readonly ClientCollection _clients;

        private readonly PacketToServiceRouter _c2sPackets;
        private readonly PacketToClientRouter _s2cPackets;
        private readonly PacketToMetaRouter _s2mPackets;

        private readonly IService[] _services;

        public PacketRouter(ClientCollection clients, PacketToServiceRouter c2sPackets, PacketToClientRouter s2cPackets, PacketToMetaRouter s2mPackets)
        {
            _clients = clients;

            _c2sPackets = c2sPackets;
            _s2cPackets = s2cPackets;
            _s2mPackets = s2mPackets;

            _services = c2sPackets.Services;
        }

        public async Task RoutePacketFromService(WrappedPacketContainer wpc, CancellationToken ct = default)
        {
            Log.Debug("Routing packet from service.");

            WrappedPacket packet = wpc.Packet;
            ClientConnection? client = _clients.GetClientById(packet.ClientId);

            if (client == null)
            {
                Log.Warning("Ignoring meta packet for unknown client {@client}", packet.ClientId);
                return;
            }

            await (wpc.IsMetaPacket switch
            {
                true => _s2mPackets.HandlePacketAsync(client, packet),
                false => _s2cPackets.HandlePacketAsync(client, packet)
            });
        }

        public Task RoutePacketFromClient(bool authenticated, WrappedPacket packet, CancellationToken ct = default)
            => _c2sPackets.DispatchAsync(authenticated, packet);
    }
}
