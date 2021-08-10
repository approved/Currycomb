using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketPong(long Timestamp) : IGamePacket
    {
        public static async Task<PacketPing> ReadAsync(Stream stream) => new(await stream.ReadLongAsync());
        public void Write(BinaryWriter writer) => writer.Write(Timestamp);
    }
}
