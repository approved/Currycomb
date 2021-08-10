using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketRequest() : IGamePacket
    {
        public static Task<PacketRequest> ReadAsync(Stream stream) => Task.FromResult<PacketRequest>(new());
    }
}
