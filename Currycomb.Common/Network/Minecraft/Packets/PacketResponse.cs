using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketResponse(String Json) : IPacket
    {
        public Task WriteAsync(Stream stream)
            => stream.WriteAsync(Json);
    }
}
