using System;
using System.IO;
using System.Text;

namespace Currycomb.Common.Network.Game.Packets
{
    public readonly struct PacketChatMessage : IGamePacket
    {
        public readonly string JsonData; // Limited to 262144 bytes.
        public readonly byte Position;
        public readonly Guid Sender;

        public PacketChatMessage(string jsonData, byte position, Guid sender)
        {
            JsonData = jsonData;
            Position = position;
            Sender = sender;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(JsonData.Length);
            writer.Write(Encoding.UTF8.GetBytes(JsonData));
            writer.Write(Position);
            writer.Write(Sender.ToByteArray());
        }
    }
}
