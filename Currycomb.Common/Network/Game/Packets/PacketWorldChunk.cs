using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    // TODO: Once the NBT library is better this should probably be updated to not use actions.
    [GamePacket(GamePacketId.WorldChunk)]
    public readonly struct PacketWorldChunk : IGamePacket
    {
        public readonly int ChunkX;
        public readonly int ChunkZ;
        public readonly long[] PrimaryBitMask;
        public readonly Action<Nbt.CompoundWriter<Nbt.Cloak>> /* Compound */ Heightmaps;
        public readonly int[] Biomes;
        public readonly byte[] Data;
        public readonly Action<Nbt.CompoundWriter<Nbt.Cloak>>[] /* Compound[] */ BlockEntities;

        public PacketWorldChunk(
            int chunkX,
            int chunkZ,
            long[] primaryBitMask,
            Action<Nbt.CompoundWriter<Nbt.Cloak>> /* Compound */ heightmaps,
            int[] biomes,
            byte[] data,
            Action<Nbt.CompoundWriter<Nbt.Cloak>>[] /* Compound[] */ blockEntities)
        {
            ChunkX = chunkX;
            ChunkZ = chunkZ;
            PrimaryBitMask = primaryBitMask;
            Heightmaps = heightmaps;
            Biomes = biomes;
            Data = data;
            BlockEntities = blockEntities;
        }

        public PacketWorldChunk(BinaryReader reader)
            => throw new NotImplementedException();

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
