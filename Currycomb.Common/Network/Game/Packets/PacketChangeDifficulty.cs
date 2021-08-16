using System.IO;
using Currycomb.Common.Game;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ChangeDifficulty)]
    public readonly struct PacketChangeDifficulty : IGamePacket
    {
        public readonly Difficulty Difficulty;
        public readonly bool IsLocked;

        public PacketChangeDifficulty(BinaryReader reader)
        {
            Difficulty = (Difficulty)reader.ReadByte();
            IsLocked = reader.ReadBoolean();
        }

        public PacketChangeDifficulty(Difficulty difficulty, bool isLocked)
        {
            Difficulty = difficulty;
            IsLocked = isLocked;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Difficulty.AsByte());
            writer.Write(IsLocked);
        }
    }
}
