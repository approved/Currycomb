using System.IO;
using System.Numerics;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.SpawnPosition)]
    public readonly struct PacketSpawnPosition : IGamePacket
    {
        public readonly Vector3 Position;
        public readonly float Angle;

        public void Write(BinaryWriter writer)
        {
            writer.Write(((((long)Position.X) & 0x3FFFFFF) << 38) | ((((long)Position.Z) & 0x3FFFFFF) << 12) | (((long)Position.Y) & 0xFFF));
            writer.Write(Angle);
        }

        public PacketSpawnPosition(Vector3 position, float angle)
        {
            Position = position;
            Angle = angle;
        }

        public PacketSpawnPosition(BinaryReader reader)
        {
            long position = reader.ReadInt64();

            Position = new Vector3(
                (float)((position >> 38) & 0x3FFFFFF),
                (float)(position & 0xFFF),
                (float)((position >> 12) & 0x3FFFFFF));

            Angle = reader.ReadSingle();
        }
    }
}
