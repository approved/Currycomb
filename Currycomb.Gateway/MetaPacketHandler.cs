using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;

namespace Currycomb.Gateway
{
    public class MetaPacketHandler
    {
        private MetaPacketRouter<MetaContext>? _router;
        public MetaPacketRouter<MetaContext> Router => _router ??= MetaPacketRouter<MetaContext>.New()
            .On<PacketSetState>(PacketSetState)
            .On<PacketSetAesKey>(PacketSetAesKey)
            .Build();

        void PacketSetState(MetaContext c, PacketSetState pkt)
            => c.SetState(pkt.State);

        void PacketSetAesKey(MetaContext c, PacketSetAesKey pkt)
            => c.SetAesKey(pkt.AesKey);
    }
}
