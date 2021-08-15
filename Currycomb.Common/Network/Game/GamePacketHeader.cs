using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Game
{
    public record GamePacketHeader(uint Length, uint PacketId)
    {
        public static GamePacketHeader Read(BinaryReader reader) => new(
            (uint)reader.Read7BitEncodedInt(),
            (uint)reader.Read7BitEncodedInt());

        public static GamePacketHeader Read(BinaryReader reader, uint length) => new(
            length,
            (uint)reader.Read7BitEncodedInt());

        public static async Task<GamePacketHeader> ReadAsync(Stream stream) => new(
            (uint)(await stream.Read7BitEncodedIntAsync()),
            (uint)(await stream.Read7BitEncodedIntAsync()));

        public static async Task<GamePacketHeader> ReadAsync(Stream stream, uint length) => new(
            length,
            (uint)(await stream.Read7BitEncodedIntAsync()));

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)Length);
            writer.Write7BitEncodedInt((int)PacketId);
        }
    }
}
