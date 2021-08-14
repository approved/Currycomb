using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    // TODO: Once the NBT library is better this should probably be updated to not require exact types.
    public record PacketChunkData(
        int ChunkX,
        int ChunkZ,
        long[] PrimaryBitMask,
        Action<Nbt.CompoundWriter<Nbt.Cloak>> /* Compound */ Heightmaps,
        int[] Biomes,
        byte[] Data,
        Action<Nbt.CompoundWriter<Nbt.Cloak>>[] /* Compound[] */ BlockEntities
    ) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            var nbt = Nbt.Writer.ToBinaryWriter(writer);

            writer.Write(ChunkX);
            writer.Write(ChunkZ);
            writer.Write7BitEncodedInt(PrimaryBitMask.Length);
            foreach (var entry in PrimaryBitMask)
                writer.Write(entry);

            nbt.Begin().WithCloak(Heightmaps).End().Finish();

            writer.Write7BitEncodedInt(Biomes.Length);
            foreach (var entry in Biomes)
                writer.Write(entry);

            writer.Write7BitEncodedInt(Data.Length);
            writer.Write(Data);
            writer.Write7BitEncodedInt(BlockEntities.Length);

            foreach (var entry in BlockEntities)
                nbt.Begin().WithCloak(entry).End().Finish();
        }
    }
}
