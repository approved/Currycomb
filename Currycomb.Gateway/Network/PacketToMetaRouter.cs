using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Meta;
using Currycomb.Gateway.Clients;
using Currycomb.Gateway.Meta;

namespace Currycomb.Gateway.Routers
{
    public class PacketToMetaRouter
    {
        private readonly MetaPacketRouter<MetaContext> _metaRouter;
        private readonly MetaPacketHandler _mph;

        public PacketToMetaRouter()
        {
            _mph = new();
            _metaRouter = _mph.Router;
        }

        public async Task HandlePacketAsync(ClientConnection client, WrappedPacket packet)
        {
            using MemoryStream memory = new(packet.GetOrCreatePacketByteArray());
            using BinaryReader reader = new(memory);

            await _metaRouter.HandlePacketAsync(new(client), reader);
        }
    }
}
