using System.IO;
using Currycomb.Common.Game.Entity;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ServerPlayerAbilities)]
    public readonly struct PacketServerPlayerAbilities : IGamePacket
    {
        private const byte CanFlyFlagPos       /**/ = 1 << 2;
        private const byte InstantBuildFlagPos /**/ = 1 << 3;
        private const byte InvulnerableFlagPos /**/ = 1 << 0;
        private const byte IsFlyingFlagPos     /**/ = 1 << 1;

        public readonly Abilities Abilities;
        public readonly float FoVMultiplier;

        public PacketServerPlayerAbilities(Abilities abilities, float fovMultiplier = 0.1f)
        {
            Abilities = abilities;
            FoVMultiplier = fovMultiplier;
        }

        public PacketServerPlayerAbilities(BinaryReader reader)
        {
            byte abilityFlags = reader.ReadByte();
            Abilities = new Abilities(
                isInvulnerable:  /**/ (abilityFlags & InvulnerableFlagPos) != 0,
                isFlying:        /**/ (abilityFlags & IsFlyingFlagPos) != 0,
                canFly:          /**/ (abilityFlags & CanFlyFlagPos) != 0,
                canInstantBuild: /**/ (abilityFlags & InstantBuildFlagPos) != 0,
                flySpeed:        /**/ reader.ReadSingle());

            FoVMultiplier = reader.ReadSingle();
        }

        public void Write(BinaryWriter writer)
        {
            byte abilityFlags = 0;
            if (Abilities.IsInvulnerable)
                abilityFlags |= InvulnerableFlagPos;

            if (Abilities.IsFlying)
                abilityFlags |= IsFlyingFlagPos;

            if (Abilities.CanFly)
                abilityFlags |= CanFlyFlagPos;

            if (Abilities.CanInstantBuild)
                abilityFlags |= InstantBuildFlagPos;

            writer.Write(abilityFlags);
            writer.Write(Abilities.FlySpeed);
            writer.Write(FoVMultiplier);
        }
    }
}
