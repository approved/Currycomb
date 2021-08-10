using System.IO;
using Serilog;

namespace Currycomb.Common.Network.Meta.Packets
{
    public record PacketSetAesKey(byte[] AesKey) : IMetaPacket
    {
        public static PacketSetAesKey Read(BinaryReader reader) => new(reader.ReadBytes(reader.Read7BitEncodedInt()));
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(AesKey.Length);
            writer.Write(AesKey);
        }
    }
}
