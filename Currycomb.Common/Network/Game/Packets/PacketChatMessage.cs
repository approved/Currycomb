using System;
using System.IO;
using System.Text;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ChatMessage)]
    public readonly struct PacketChatMessage : IGamePacket
    {
        public readonly string JsonData; // Limited to 262144 bytes.
        public readonly byte Position;
        public readonly Guid Sender;

        public PacketChatMessage(BinaryReader reader)
        {
            JsonData = reader.ReadString();
            Position = reader.ReadByte();
            Sender = new Guid(reader.ReadBytes(16));
        }

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
