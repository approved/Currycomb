using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketEntityEvent(int EntityId, byte Status) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(EntityId);
            writer.Write(Status);
        }
    }
}
