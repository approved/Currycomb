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

            Nbt.Writer.ToBinaryWriter(writer)
                .Compound(x => x
                    .Compound("minecraft:dimension_type", x => x
                        .Write("type", "minecraft:dimension_type")
                        .ListCompound("value", 1, x => x
                            .Compound(x => x
                                .Write("name", "minecraft:overworld")
                                .Write("id", 0)
                                .Compound("element", x => x.WithCloak(dimension))))));

            Nbt.Writer.ToBinaryWriter(writer)
                .Compound(x => x.WithCloak(dimension));

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
