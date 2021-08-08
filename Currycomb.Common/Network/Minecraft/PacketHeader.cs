using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft
{
    public record PacketHeader(uint Length, uint PacketId)
    {
        public static async Task<PacketHeader> ReadAsync(Stream stream) => new(
            (uint)(await stream.Read7BitEncodedIntAsync()),
            (uint)(await stream.Read7BitEncodedIntAsync())
        );

        public static async Task<PacketHeader> ReadAsync(Stream stream, uint length) => new(
            length,
            (uint)(await stream.Read7BitEncodedIntAsync())
        );

        public async Task WriteAsync(Stream stream)
        {
            await stream.Write7BitEncodedIntAsync((int)Length);
            await stream.Write7BitEncodedIntAsync((int)PacketId);
        }
    }
}
