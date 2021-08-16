using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.Ping)]
    public readonly struct PacketPing : IGamePacket
    {
        public readonly long Timestamp;

        public PacketPing(BinaryReader stream)
        {
            Timestamp = stream.ReadInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Timestamp);
        }
    }
}
