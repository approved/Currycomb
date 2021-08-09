using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketHandshake(uint ProtocolVersion, String ServerAddress, ushort Port, State State) : IPacket
    {
        public static async Task<PacketHandshake> ReadAsync(Stream stream) => new(
            await stream.Read7BitEncodedUIntAsync(),
            await stream.ReadStringAsync(),
            await stream.ReadUShortAsync(),
            StateExt.FromRaw(await stream.Read7BitEncodedUIntAsync())
        );
    }
}
