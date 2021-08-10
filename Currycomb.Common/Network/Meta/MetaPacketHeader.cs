using System.IO;

namespace Currycomb.Common.Network.Meta
{
    public record MetaPacketHeader(byte PacketId)
    {
        public static MetaPacketHeader Read(BinaryReader reader) => new((byte)reader.ReadByte());
        public void Write(BinaryWriter writer) => writer.Write(PacketId);
    }
}
