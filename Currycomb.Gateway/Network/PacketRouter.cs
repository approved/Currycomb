using System.Threading;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.Gateway.Clients;
using Serilog;

namespace Currycomb.Gateway.Routers
{
    public class PacketRouter
    {
        private readonly ClientCollection _clients;
        private readonly PacketToMetaRouter _toMeta;
        private readonly PacketToServiceRouter _toService;

        public PacketRouter(ClientCollection clients, PacketToServiceRouter toService, PacketToMetaRouter toMeta)
        {
            _clients = clients;

            _toService = toService;
            _toMeta = toMeta;
        }

        public ValueTask RoutePacketFromClient(State state, WrappedPacket packet, CancellationToken ct = default)
            => _toService.HandleGameAsync(BoundTo.Server, state, packet);

        public async Task RoutePacketFromService(WrappedPacketContainer wpc, CancellationToken ct = default)
        {
            Log.Debug("Routing packet from service.");

            WrappedPacket packet = wpc.Packet;

            if (wpc.IsMetaPacket)
            {
                if (_clients.GetClientById(packet.ClientId) is ClientConnection client)
                {
                    Log.Information("Handling meta packet for client {@client}", packet.ClientId);
                    await _toMeta.HandlePacketAsync(client, packet);

                    Log.Information("Dispatching meta packet for client {@client}", packet.ClientId);
                    await _toService.HandleMetaAsync(packet);

                    Log.Information("Done with meta packet for client {@client}", packet.ClientId);
                }
                else
                {
                    Log.Warning("Ignoring packet for unknown client {@client}", packet.ClientId);
                }
            }
            else
            {
                Log.Information("Handling game packet for client {@client}", packet.ClientId);
                await _clients.SendPacketAsync(packet.ClientId, packet);
            }
        }
    }
}
