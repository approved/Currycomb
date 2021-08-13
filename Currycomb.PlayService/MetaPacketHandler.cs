using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;
using System.Threading.Tasks;

namespace Currycomb.PlayService
{
    public class MetaPacketHandler
    {
        MetaPacketRouter<Context>? _router;
        public MetaPacketRouter<Context> Router => _router ??= MetaPacketRouter<Context>.New()
            .On<PacketSetState>(PacketSetState)
            .Build();

        private async Task PacketSetState(Context c, PacketSetState pkt)
        {
            if (pkt.State == State.Play)
            {
                await c.Event.WriteAsync(new EvtPlayerConnected(c.ClientId));
            }
        }
    }
}
