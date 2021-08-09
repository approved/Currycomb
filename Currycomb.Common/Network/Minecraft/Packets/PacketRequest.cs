using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketRequest() : IPacket
    {
        public static Task<PacketRequest> ReadAsync(Stream stream) => Task.FromResult<PacketRequest>(new());
    }
}
