using Currycomb.Common.Game;
using Currycomb.Nbt;
using System;
using System.IO;

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
        int MaxPlayers,
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
                writer.Write(worldName);

            Action<Nbt.CompoundWriter<Nbt.Cloak>> dimension = x => x
                .Write("piglin_safe", (byte)0)
                .Write("natural", (byte)1)
                .Write("ambient_light", 0.0f)
                .Write("infiniburn", "minecraft:infiniburn_overworld")
                .Write("respawn_anchor_works", 0)
                .Write("has_skylight", 1)
                .Write("bed_works", 1)
                .Write("effects", "minecraft:overworld")
                .Write("has_raids", 1)
                .Write("min_y", 0)
                .Write("height", 256)
                .Write("logical_height", 256)
                .Write("coordinate_scale", 1.0d)
                .Write("ultrawarm", 0)
                .Write("has_ceiling", 0);

            Action<Nbt.CompoundWriter<Nbt.Cloak>> plainsBiome = x => x
            .Write("precipitation", "rain")
            .Compound("effects", x => x
                .Write("sky_color", 0x6EB1FF)
                .Write("water_fog_color", 0x50533)
                .Write("fog_color", 0xC0D8FF)
                .Write("water_color", 0x3F76E4)
                .Compound("mood_sound", x => x
                    .Write("tick_delay", 6000)
                    .Write("offset", 2.0d)
                    .Write("sound", "minecraft:ambient.cave")
                    .Write("block_search_extent", 8)))
                .Write("depth", 0.125f)
                .Write("temperature", 2.0f)
                .Write("scale", 0.05f)
                .Write("downfall", 0.0f)
                .Write("category", "plains");

            Nbt.Writer.ToBinaryWriter(writer)
                .Compound(x => x
                    .Compound("minecraft:dimension_type", x => x
                        .Write("type", "minecraft:dimension_type")
                        .ListCompound("value", 1, x => x
                            .Compound(x => x
                                .Write("name", "minecraft:overworld")
                                .Write("id", 0)
                                .Compound("element", x => x.WithCloak(dimension)))))
                    .Compound("minecraft:worldgen/biome", x => x
                        .Write("type", "minecraft:worldgen/biome")
                        .ListCompound("value", 1, x => x
                            .Compound(x => x
                                .Write("name", "minecraft:plains")
                                .Write("id", 0)
                                .Compound("element", x => x.WithCloak(plainsBiome))))));

            Nbt.Writer.ToBinaryWriter(writer)
                .Compound(x => x.WithCloak(dimension));

            writer.Write(SpawnWorldIdentifier);
            //writer.Write(BitConverter.GetBytes(WorldSeed));
            writer.Write(new byte[] { 0x83, 0xAA, 0x66, 0xF8, 0x2B, 0x9C, 0x81, 0xD2 });
            writer.Write7BitEncodedInt(MaxPlayers);
            writer.Write7BitEncodedInt(RenderDistance);
            writer.Write(ReducedDebugInfo);
            writer.Write(EnableRespawnScreen);
            writer.Write(IsDebug);
            writer.Write(IsFlat);
        }
    }
}
