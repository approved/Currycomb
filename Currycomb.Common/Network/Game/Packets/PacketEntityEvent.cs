using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.EntityEvent)]
    public readonly struct PacketEntityEvent : IGamePacket
    {
        public readonly int EntityId;
        public readonly byte Status;

        public PacketEntityEvent(int entityId, byte status)
        {
            EntityId = entityId;
            Status = status;
        }

        public PacketEntityEvent(BinaryReader reader)
        {
            EntityId = reader.ReadInt32();
            Status = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(EntityId);
            writer.Write(Status);
        }
    }
}
