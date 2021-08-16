using System;
using System.IO;
using Currycomb.Common.Game;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ContainerSetContent)]
    public readonly struct PacketSetContainer : IGamePacket
    {
        public readonly byte WindowId;
        public readonly int StateId;
        public readonly InventorySlot[] SlotData;
        public readonly InventorySlot CarriedItem;

        public PacketSetContainer(byte windowId, int stateId, InventorySlot[] slotData, InventorySlot carriedItem)
        {
            WindowId = windowId;
            StateId = stateId;
            SlotData = slotData;
            CarriedItem = carriedItem;
        }

        public PacketSetContainer(BinaryReader reader)
            => throw new NotImplementedException();

        public void Write(BinaryWriter writer)
        {
            writer.Write(WindowId);
            writer.Write7BitEncodedInt(StateId);
            writer.Write7BitEncodedInt(SlotData.Length);
            foreach (var item in SlotData)
                item.Write(writer);

            CarriedItem.Write(writer);
        }
    }
}
