using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ServerKeepAlive)]
    public readonly struct PacketServerKeepAlive : IGamePacket
    {
        public readonly ulong KeepAliveId;

        public PacketServerKeepAlive(ulong keepAliveId)
        {
            KeepAliveId = keepAliveId;
        }

        public PacketServerKeepAlive(BinaryReader reader)
        {
            KeepAliveId = reader.ReadUInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(KeepAliveId);
        }
    }

}
