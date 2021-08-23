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
        public readonly long[] StripBitMask;
        public readonly Action<Nbt.CompoundWriter<Nbt.Cloak>> /* Compound */ Heightmaps;
        public readonly int[] Biomes;
        public readonly byte[] Data;
        public readonly Action<Nbt.CompoundWriter<Nbt.Cloak>>[] /* Compound[] */ BlockEntities;

        public PacketWorldChunk(
            int chunkX,
            int chunkZ,
            long[] stripBitMask,
            Action<Nbt.CompoundWriter<Nbt.Cloak>> /* Compound */ heightmaps,
            int[] biomes,
            byte[] data,
            Action<Nbt.CompoundWriter<Nbt.Cloak>>[] /* Compound[] */ blockEntities)
        {
            ChunkX = chunkX;
            ChunkZ = chunkZ;
            StripBitMask = stripBitMask;
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

            // flip endianness of 32 bit int chunkx
            var chunkXFlipped = (ChunkX << 24) | ((ChunkX & 0xFF00) << 8) | ((ChunkX & 0xFF0000) >> 8) | (ChunkX >> 24);
            var chunkYFlipped = (ChunkZ << 24) | ((ChunkZ & 0xFF00) << 8) | ((ChunkZ & 0xFF0000) >> 8) | (ChunkZ >> 24);

            writer.Write(chunkXFlipped);
            writer.Write(chunkYFlipped);
            writer.Write7BitEncodedInt(StripBitMask.Length);
            for (var i = 0; i < StripBitMask.Length; i++)
                writer.Write(StripBitMask[i]);

            nbt.Begin().WithCloak(Heightmaps).End().Finish();

            writer.Write7BitEncodedInt(Biomes.Length);
            foreach (var entry in Biomes)
                writer.Write7BitEncodedInt(entry);

            writer.Write7BitEncodedInt(Data.Length);
            writer.Write(Data);

            writer.Write7BitEncodedInt(BlockEntities.Length);
            foreach (var entry in BlockEntities)
                nbt.Begin().WithCloak(entry).End().Finish();
        }
    }
}
