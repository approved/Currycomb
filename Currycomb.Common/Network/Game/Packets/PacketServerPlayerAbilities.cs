using Currycomb.Common.Game.Entity;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketServerPlayerAbilities(Abilities Abilities, float FoVMultiplier = 0.1f) : IGamePacket
    {
        private const byte InvulnerableFlagPos = 1 << 0;
        private const byte IsFlyingFlagPos     = 1 << 1;
        private const byte CanFlyFlagPos       = 1 << 2;
        private const byte InstantBuildFlagPos = 1 << 3;

        public void Write(BinaryWriter writer)
        {
            byte abilityFlags = 0;
            if (Abilities.IsInvulnerable)
            {
                abilityFlags |= InvulnerableFlagPos;
            }

            if (Abilities.IsFlying)
            {
                abilityFlags |= IsFlyingFlagPos;
            }

            if (Abilities.CanFly)
            {
                abilityFlags |= CanFlyFlagPos;
            }

            if (Abilities.CanInstantBuild)
            {
                abilityFlags |= InstantBuildFlagPos;
            }

            writer.Write(abilityFlags);
            writer.Write(Abilities.FlySpeed);
            writer.Write(FoVMultiplier);
        }
    }
}
