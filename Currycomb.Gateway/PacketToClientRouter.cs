using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Gateway.ClientData;

namespace Currycomb.Gateway
{
    public class PacketToClientRouter
    {
        public Task HandlePacketAsync(ClientConnection client, WrappedPacket packet)
            => client.SendPacketAsync(packet.Data);
    }
}
