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

        private readonly PacketToServiceRouter _toService;
        private readonly PacketToClientRouter _toClient;
        private readonly PacketToMetaRouter _toMeta;

        private readonly IService[] _services;

        public PacketRouter(ClientCollection clients, PacketToServiceRouter toService, PacketToClientRouter toClient, PacketToMetaRouter toMeta)
        {
            _clients = clients;

            _toService = toService;
            _toClient = toClient;
            _toMeta = toMeta;

            _services = toService.Services;
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

            if (wpc.IsMetaPacket)
            {
                Log.Information("Handling meta packet for client {@client}", packet.ClientId);
                await _toMeta.HandlePacketAsync(client, packet);

                Log.Information("Dispatching meta packet for client {@client}", packet.ClientId);
                await _toService.HandleMetaAsync(packet);

                Log.Information("Done with meta packet for client {@client}", packet.ClientId);
            }
            else
            {
                Log.Information("Handling game packet for client {@client}", packet.ClientId);
                await _toClient.HandlePacketAsync(client, packet);
            }
        }

        public ValueTask RoutePacketFromClient(bool authenticated, WrappedPacket packet, CancellationToken ct = default)
            => _toService.HandleGameAsync(authenticated, packet);
    }
}
