﻿using System.IO;
using System.Numerics;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketSpawnPosition(Vector3 Position, float Angle) : IPacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(((((long)Position.X) & 0x3FFFFFF) << 38) | ((((long)Position.Z) & 0x3FFFFFF) << 12) | (((long)Position.Y) & 0xFFF));
            writer.Write(Angle);
        }
    }
}
