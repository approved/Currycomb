using System;
using System.IO;
using Currycomb.Common.Game.Entity;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ClientPlayerAbilities)]
    public readonly struct PacketClientPlayerAbilities : IGamePacket
    {
        public readonly Abilities Abilities;

        // TODO: What are the other flags?
        private const byte IsFlyingFlagPos = 1 << 1;

        public PacketClientPlayerAbilities(Abilities abilities)
        {
            Abilities = abilities;
        }

        public PacketClientPlayerAbilities(BinaryReader reader)
        {
            throw new NotImplementedException();
        }

        public void Write(BinaryWriter writer)
        {
            byte abilityFlags = 0;

            if (Abilities.IsFlying)
                abilityFlags |= IsFlyingFlagPos;

            writer.Write(abilityFlags);
        }
    }
}
