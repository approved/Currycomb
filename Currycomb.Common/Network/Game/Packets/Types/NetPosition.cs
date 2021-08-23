using System.IO;
using System.Numerics;

namespace Currycomb.Common.Network.Game.Packets.Types
{
    public readonly struct NetPosition
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public NetPosition(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public NetPosition(Vector3 vector)
        {
            X = (int)vector.X;
            Y = (int)vector.Y;
            Z = (int)vector.Z;
        }

        public NetPosition(BinaryReader reader)
        {
            var value = reader.ReadInt64();
            X = (int)(value >> 38) & 0x3FFFFFF;
            Z = (int)(value >> 12) & 0x3FFFFFF;
            Y = (int)(value >> 00) & 0xFFF;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(
                (((long)X & 0x3FFFFFF) << 38) |
                (((long)Z & 0x3FFFFFF) << 12) |
                ((long)Y & 0xFFF));
        }
    }
}