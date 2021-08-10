using System;
using System.IO;
using Currycomb.Common.Network.Meta.Packets;

namespace Currycomb.Common.Network.Meta
{

    public static class MetaPacketReader
    {
        public static IMetaPacket Read(MetaPacketId id, BinaryReader reader) => id switch
        {
            MetaPacketId.SetState => PacketSetState.Read(reader),
            MetaPacketId.SetAesKey => PacketSetAesKey.Read(reader),
            _ => throw new NotSupportedException($"Packet not yet supported: {id}"),
        };
    }
}
