using Currycomb.Common.Game;
using System.IO;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketChangeDifficulty(Difficulty Difficulty, bool IsLocked)
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(Difficulty.AsByte());
            writer.Write(IsLocked);
        }
    }
}
