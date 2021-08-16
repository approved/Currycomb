using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.SetHeldItem)]
    public readonly struct PacketSetHeldItem : IGamePacket
    {
        public readonly byte Slot;

        public PacketSetHeldItem(byte slot)
        {
            Slot = slot;
        }

        public PacketSetHeldItem(BinaryReader reader)
        {
            Slot = reader.ReadByte();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Slot);
        }
    }
}
