using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types.Command.Properties
{
    public readonly struct StringCommandProperties : ICommandProperties
    {
        public enum StringType : int
        {
            SingleWord = 0, // Reads a single word 
            QuotablePhrase = 1, // If it starts with a ", keeps reading until another " (allowing escaping with \). Otherwise behaves the same as SINGLE_WORD
            GreedyPhrase = 2, // Reads the rest of the content after the cursor. Quotes will not be removed.
        }

        public readonly StringType Type;

        public StringCommandProperties(StringType type)
        {
            Type = type;
        }

        public StringCommandProperties(BinaryReader reader)
        {
            Type = (StringType)reader.Read7BitEncodedInt();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)Type);
        }
    }
}