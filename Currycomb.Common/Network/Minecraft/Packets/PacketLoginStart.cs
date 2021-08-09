using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketLoginStart(String Username) : IPacket
    {
        public static async Task<PacketLoginStart> ReadAsync(Stream stream) => new(
            await stream.ReadStringAsync()
        );
    }
}
