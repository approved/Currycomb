using Currycomb.Common.Game;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketChangeDifficulty(Difficulty Difficulty, bool IsLocked) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(Difficulty.AsByte());
            writer.Write(IsLocked);
        }
    }
}
