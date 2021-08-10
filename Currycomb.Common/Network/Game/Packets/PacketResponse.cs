using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketResponse(String Json) : IGamePacket
    {
        public void Write(BinaryWriter stream)
            => stream.Write(Json);
    }
}
