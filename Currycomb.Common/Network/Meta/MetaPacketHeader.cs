using System.IO;

namespace Currycomb.Common.Network.Meta
{
    public readonly struct MetaPacketHeader
    {
        public readonly MetaPacketId PacketId { get; init; }

        public MetaPacketHeader(MetaPacketId packetId)
            => PacketId = packetId;

        public static MetaPacketHeader Read(BinaryReader reader)
            => new((MetaPacketId)reader.ReadByte());

        public void Write(BinaryWriter writer)
            => writer.Write((byte)PacketId);
    }
}
