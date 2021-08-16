using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.Pong)]
    public readonly struct PacketPong : IGamePacket
    {
        public readonly long Timestamp;

        public PacketPong(long timestamp)
        {
            Timestamp = timestamp;
        }

        public PacketPong(BinaryReader reader)
        {
            Timestamp = reader.ReadInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Timestamp);
        }
    }
}
