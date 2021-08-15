using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Gateway.Clients;
using Serilog;

namespace Currycomb.Gateway.Routers
{
    public class PacketToClientRouter
    {
        public Task HandlePacketAsync(ClientConnection client, WrappedPacket packet)
        {
            Log.Information("Sending Client Data: {packet}", packet);
            return client.SendPacketAsync(packet.Data);
        }
    }
}
