using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketLoginSuccess(Guid Uuid, String Username) : IPacket
    {
        public async Task WriteAsync(Stream stream)
        {
            await stream.WriteAsync(Uuid);
            await stream.WriteAsync(Username);
        }
    }
}
