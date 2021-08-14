using System.IO;
using Currycomb.Common.Game;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketSetContainer(byte WindowId, int StateId, InventorySlot[] SlotData, InventorySlot CarriedItem) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(WindowId);
            writer.Write7BitEncodedInt(StateId);
            writer.Write7BitEncodedInt(SlotData.Length);
            foreach (var item in SlotData)
            {
                item.Write(writer);
            }
            CarriedItem.Write(writer);
        }
    }
}
