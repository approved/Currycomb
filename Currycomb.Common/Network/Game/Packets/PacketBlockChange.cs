using System.IO;
using Currycomb.Common.Network.Game.Packets.Types;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.BlockUpdate)]
    public readonly struct PacketBlockUpdate : IGamePacket
    {
        public readonly NetPosition Location;
        public readonly int BlockId;

        public PacketBlockUpdate(NetPosition location, int blockId)
        {
            Location = location;
            BlockId = blockId;
        }

        public PacketBlockUpdate(BinaryReader reader)
        {
            Location = new NetPosition(reader);
            BlockId = reader.Read7BitEncodedInt();
        }

        public void Write(BinaryWriter writer)
        {
            Location.Write(writer);
            writer.Write7BitEncodedInt(BlockId);
        }
    }
}
