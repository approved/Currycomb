using Currycomb.Common.Game;
using fNbt;
using System.IO;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketJoinGame(
        int EntityID,
        bool IsHardcore,
        GameMode GameMode,
        GameMode PreviousGameMode,
        string[] WorldNames,
        // DimensionCodec,
        // Dimension
        string SpawnWorldIdentifier,
        long WorldSeed,
        int RenderDistance,
        bool ReducedDebugInfo,
        bool EnableRespawnScreen,
        bool IsDebug,
        bool IsFlat) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(EntityID);
            writer.Write(IsHardcore);
            writer.Write(GameMode.AsByte());
            writer.Write(PreviousGameMode.AsByte());
            writer.Write7BitEncodedInt(WorldNames.Length);
            foreach (string worldName in WorldNames)
            {
                writer.Write(worldName);
            }

            {
                var nbt = new NbtWriter(writer.BaseStream, string.Empty);
                {
                    nbt.BeginCompound("minecraft:dimension_type");
                    {
                        nbt.WriteString("type", "minecraft:dimension_type");
                        nbt.BeginList("value", NbtTagType.Compound, 1);
                        nbt.BeginCompound();
                        {
                            nbt.WriteString("name", "minecraft:overworld");
                            nbt.WriteInt("id", 0);
                            nbt.BeginCompound("element");
                            {
                                nbt.WriteByte("piglin_safe", 0);
                                nbt.WriteByte("natural", 1);
                                nbt.WriteFloat("ambient_light", 0.0f);
                                nbt.WriteString("infiniburn", "minecraft:infiniburn_overworld");
                                nbt.WriteByte("respawn_anchor_works", 0);
                                nbt.WriteByte("has_skylight", 1);
                                nbt.WriteByte("bed_works", 1);
                                nbt.WriteString("effects", "minecraft:overworld");
                                nbt.WriteByte("has_raids", 1);
                                nbt.WriteInt("min_y", 0);
                                nbt.WriteInt("height", 256);
                                nbt.WriteInt("logical_height", 256);
                                nbt.WriteDouble("coordinate_scale", 1.0d);
                                nbt.WriteByte("ultrawarm", 0);
                                nbt.WriteByte("has_ceiling", 0);
                            }
                            nbt.EndCompound();
                        }
                        nbt.EndCompound();
                        nbt.EndList();
                    }
                    nbt.EndCompound();
                }
                nbt.EndCompound();
                nbt.Finish();
            }

            {
                var nbt = new NbtWriter(writer.BaseStream, string.Empty);
                nbt.WriteByte("piglin_safe", 0);
                nbt.WriteByte("natural", 1);
                nbt.WriteFloat("ambient_light", 0.0f);
                nbt.WriteString("infiniburn", "minecraft:infiniburn_overworld");
                nbt.WriteByte("respawn_anchor_works", 0);
                nbt.WriteByte("has_skylight", 1);
                nbt.WriteByte("bed_works", 1);
                nbt.WriteString("effects", "minecraft:overworld");
                nbt.WriteByte("has_raids", 1);
                nbt.WriteInt("min_y", 0);
                nbt.WriteInt("height", 256);
                nbt.WriteInt("logical_height", 256);
                nbt.WriteDouble("coordinate_scale", 1.0d);
                nbt.WriteByte("ultrawarm", 0);
                nbt.WriteByte("has_ceiling", 0);
                nbt.EndCompound();
                nbt.Finish();
            }

            writer.Write(SpawnWorldIdentifier);
            writer.Write(WorldSeed);
            writer.Write7BitEncodedInt(0);
            writer.Write7BitEncodedInt(RenderDistance);
            writer.Write(ReducedDebugInfo);
            writer.Write(EnableRespawnScreen);
            writer.Write(IsDebug);
            writer.Write(IsFlat);
        }
    }
}
