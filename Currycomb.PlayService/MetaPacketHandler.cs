using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;
using Currycomb.PlayService.ExternalEvent;
using Serilog;
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
            Log.Debug("PacketSetState | {@pkt}", pkt);
            if (pkt.State == State.Play)
                await c.Event.WriteAsync(new ClientConnected(c.ClientId));
        }
    }
}
