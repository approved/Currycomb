using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketSetHeldItem(byte Slot) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(Slot);
        }
    }
}
