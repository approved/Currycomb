using System.IO;

namespace Currycomb.Common.Game
{
    public struct InventorySlot
    {
        // TODO: This struct is not ready to be used for anything other than Present = False.

        public struct Todo { }

        public bool Present;
        public int ItemId;
        public int Count;
        public Todo Nbt;

        public void Write(BinaryWriter writer)
        {
            writer.Write(Present);
            if (Present)
            {
                writer.Write7BitEncodedInt(ItemId);
                writer.Write(Count);
                // writer.Write(Nbt);
            }
        }
    }
}
