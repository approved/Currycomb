using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ServerKeepAlive)]
    public readonly struct PacketClientKeepAlive : IGamePacket
    {
        public readonly ulong KeepAliveId;

        public PacketClientKeepAlive(ulong keepAliveId)
        {
            KeepAliveId = keepAliveId;
        }

        public PacketClientKeepAlive(BinaryReader reader)
        {
            KeepAliveId = reader.ReadUInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(KeepAliveId);
        }
    }
}
