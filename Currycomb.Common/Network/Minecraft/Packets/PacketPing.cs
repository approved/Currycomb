using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketPing(long Timestamp) : IPacket
    {
        public static async Task<PacketPing> ReadAsync(Stream stream) => new(await stream.ReadLongAsync());
        public Task WriteAsync(Stream stream) => stream.WriteAsync(Timestamp);
    }
}
