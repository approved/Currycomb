using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public readonly struct EntityCommandProperties : ICommandProperties
    {
        public enum EntityTypeFlags : byte
        {
            None = 0,
            OnlySingleEntity = 1,
            OnlyPlayers = 2,
        }

        public readonly EntityTypeFlags Flags;

        public EntityCommandProperties(EntityTypeFlags flags)
        {
            Flags = flags;
        }

        public EntityCommandProperties(BinaryReader reader)
        {
            Flags = (EntityTypeFlags)reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Flags);
        }
    }
}