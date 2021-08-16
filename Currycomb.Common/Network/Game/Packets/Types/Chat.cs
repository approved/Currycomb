using System.IO;

namespace Currycomb.Common.Network.Game.Packets.Types
{
    public readonly struct Chat
    {
        public readonly string Text;

        public Chat(string text)
        {
            Text = text;
        }

        public Chat(BinaryReader reader)
        {
            Text = reader.ReadString();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write($"{{\"text\":\"{Text}\"}}");
        }
    }
}
