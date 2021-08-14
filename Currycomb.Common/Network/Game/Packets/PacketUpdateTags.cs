using Currycomb.Common.Game.Tags;
using System.IO;
using System.Runtime.CompilerServices;

namespace Currycomb.Common.Network.Game.Packets
{
    public class PacketUpdateTags : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(5);
            RequiredTags.Block.WriteDefault(writer);
            RequiredTags.Item.WriteDefault(writer);
            RequiredTags.Fluid.WriteDefault(writer);
            RequiredTags.EntityType.WriteDefault(writer);
            RequiredTags.GameEvent.WriteDefault(writer);
        }
    }

    public enum RequiredTags
    {
        Block,
        Item,
        Fluid,
        EntityType,
        GameEvent
    }

    public static class RequiredTagsExt
    {
        public static string GetString(this RequiredTags tag) => tag switch
        {
            RequiredTags.Block => "minecraft:block",
            RequiredTags.Item => "minecraft:item",
            RequiredTags.Fluid => "minecraft:fluid",
            RequiredTags.EntityType => "minecraft:entity_type",
            RequiredTags.GameEvent => "minecraft:game_event",
            _ => "currycomb:unknown"
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteDefault(this RequiredTags tag, BinaryWriter writer)
        {
            writer.Write(tag.GetString());
            switch (tag)
            {
                case RequiredTags.Block:
                    {
                        writer.Write7BitEncodedInt(BlockTypeTags.RequiredTags.Count);
                        foreach (string item in BlockTypeTags.RequiredTags)
                        {
                            writer.Write(item);
                            writer.Write7BitEncodedInt(0);
                        }
                        break;
                    }

                case RequiredTags.Item:
                    {
                        writer.Write7BitEncodedInt(ItemTags.RequiredTags.Count);
                        foreach (string item in ItemTags.RequiredTags)
                        {
                            writer.Write(item);
                            writer.Write7BitEncodedInt(0);
                        }
                        break;
                    }

                case RequiredTags.Fluid:
                    {
                        writer.Write7BitEncodedInt(FluidTypeTags.RequiredTags.Count);
                        foreach (string item in FluidTypeTags.RequiredTags)
                        {
                            writer.Write(item);
                            writer.Write7BitEncodedInt(0);
                        }
                        break;
                    }

                case RequiredTags.EntityType:
                    {
                        writer.Write7BitEncodedInt(EntityTypeTags.RequiredTags.Count);
                        foreach (string item in EntityTypeTags.RequiredTags)
                        {
                            writer.Write(item);
                            writer.Write7BitEncodedInt(0);
                        }
                        break;
                    }

                case RequiredTags.GameEvent:
                    {
                        writer.Write7BitEncodedInt(GameEventTags.RequiredTags.Count);
                        foreach (string item in GameEventTags.RequiredTags)
                        {
                            writer.Write(item);
                            writer.Write7BitEncodedInt(0);
                        }
                        break;
                    }
            }
        }
    }
}
