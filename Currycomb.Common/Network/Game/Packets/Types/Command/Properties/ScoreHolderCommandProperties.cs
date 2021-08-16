using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public readonly struct ScoreHolderCommandProperties : ICommandProperties
    {
        public enum ScoreHolderFlags : byte
        {
            None = 0,
            AllowMultiple = 1,
        }

        public readonly ScoreHolderFlags Flags;

        public ScoreHolderCommandProperties(ScoreHolderFlags flags)
        {
            Flags = flags;
        }

        public ScoreHolderCommandProperties(BinaryReader reader)
        {
            Flags = (ScoreHolderFlags)reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write((byte)Flags);
        }
    }
}