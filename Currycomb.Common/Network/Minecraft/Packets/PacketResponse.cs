using System;
using System.IO;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketResponse(String Json) : IPacket
    {
        public void Write(BinaryWriter stream)
            => stream.Write(Json);
    }
}
