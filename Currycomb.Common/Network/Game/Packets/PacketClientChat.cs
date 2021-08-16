using System.IO;
using System.Text;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ClientChat)]
    public readonly struct PacketClientChat : IGamePacket
    {
        public readonly string Message;

        public PacketClientChat(string message)
        {
            Message = message;
        }

        public PacketClientChat(BinaryReader reader)
        {
            Message = Encoding.UTF8.GetString(reader.ReadBytes(reader.Read7BitEncodedInt()));
        }
    }
}
