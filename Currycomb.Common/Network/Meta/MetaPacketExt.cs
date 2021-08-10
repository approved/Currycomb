using System;
using System.IO;

namespace Currycomb.Common.Network.Meta
{
    // TODO: Get rid of this
    public static class MetaPacketExt
    {
        public static Memory<byte> ToBytes<T>(this T packet) where T : IMetaPacket
            => ToBytes(packet, MetaPacketIdMap<T>.Id);

        public static Memory<byte> ToBytes<T>(this T packet, MetaPacketId id) where T : IMetaPacket
        {
            using MemoryStream ms = new();
            using BinaryWriter bw = new(ms);

            bw.Write(id.ToRaw());
            packet.Write(bw);

            return new Memory<byte>(ms.ToArray());
        }
    }
}
