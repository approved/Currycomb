using Currycomb.Common.Game.Entity;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketClientPlayerAbilities(Abilities Abilities, float FoVMultiplier) : IGamePacket
    {
        private const byte IsFlyingFlagPos = 1 << 1;

        public void Write(BinaryWriter writer)
        {
            byte abilityFlags = 0;
            if (Abilities.IsFlying)
            {
                abilityFlags |= IsFlyingFlagPos;
            }

            writer.Write(abilityFlags);
        }
    }
}
