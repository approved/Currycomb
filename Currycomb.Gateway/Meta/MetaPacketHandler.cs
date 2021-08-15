using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;

namespace Currycomb.Gateway.Meta
{
    public class MetaPacketHandler
    {
        private MetaPacketRouter<MetaContext>? _router;
        public MetaPacketRouter<MetaContext> Router => _router ??= MetaPacketRouter<MetaContext>.New()
            .On<PacketSetAesKey>(PacketSetAesKey)
            .On<PacketSetState>(PacketSetState)
            .Build();

        void PacketSetAesKey(MetaContext c, PacketSetAesKey pkt)
            => c.SetAesKey(pkt.AesKey);

        void PacketSetState(MetaContext c, PacketSetState pkt)
            => c.SetState(pkt.State);
    }
}
