using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Gateway.ClientData;
using Currycomb.Common.Network.Meta;
using System.IO;

namespace Currycomb.Gateway
{
    public class PacketToMetaRouter
    {
        private readonly MetaPacketHandler _mph;
        private readonly MetaPacketRouter<MetaContext> _metaRouter;

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
