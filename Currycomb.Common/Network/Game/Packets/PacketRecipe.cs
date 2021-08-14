using Currycomb.Common.Network.Game.Packets.Types;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketRecipe(RecipePacketState State, RecipeBooks RecipeBooks) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt((int)State);
            writer.Write(RecipeBooks.CraftingSettings.Open);
            writer.Write(RecipeBooks.CraftingSettings.IsFiltering);
            writer.Write(RecipeBooks.FurnaceSettings.Open);
            writer.Write(RecipeBooks.FurnaceSettings.IsFiltering);
            writer.Write(RecipeBooks.BlastFurnaceSettings.Open);
            writer.Write(RecipeBooks.BlastFurnaceSettings.IsFiltering);
            writer.Write(RecipeBooks.SmokerSettings.Open);
            writer.Write(RecipeBooks.SmokerSettings.IsFiltering);

            // TODO: Implement Writing Recipes out if the user has any
            writer.Write7BitEncodedInt(0);
            writer.Write7BitEncodedInt(0);
        }
    }

    public enum RecipePacketState
    {
        Init,
        Add,
        Remove,
    }

    public enum RecipeBookType
    {
        Crafting,
        Furnace,
        BlastFurnace,
        Smoker
    }

    public record RecipeBookSettings(RecipeBookType Type, bool Open, bool IsFiltering);

    public class RecipeBooks
    {
        public RecipeBookSettings CraftingSettings = new(RecipeBookType.Crafting, false, false);
        public RecipeBookSettings FurnaceSettings = new(RecipeBookType.Furnace, false, false);
        public RecipeBookSettings BlastFurnaceSettings = new(RecipeBookType.BlastFurnace, false, false);
        public RecipeBookSettings SmokerSettings = new(RecipeBookType.Smoker, false, false);
    }
}
