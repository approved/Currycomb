using System.IO;
using fNbt;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketWorldChunk(int ChunkX, int ChunkZ, long[] PrimaryBitMask, NbtCompound Heightmaps, int[] Biomes, byte[] Data, NbtCompound[] BlockEntities) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(ChunkX);
            writer.Write(ChunkZ);
            writer.Write7BitEncodedInt(PrimaryBitMask.Length);
            foreach (var entry in PrimaryBitMask)
            {
                writer.Write(entry);
            }
            new NbtFile(Heightmaps).SaveToStream(writer.BaseStream, NbtCompression.None);
            writer.Write7BitEncodedInt(Biomes.Length);
            foreach (var entry in Biomes)
            {
                writer.Write(entry);
            }
            writer.Write7BitEncodedInt(Data.Length);
            writer.Write(Data);
            writer.Write7BitEncodedInt(BlockEntities.Length);
            foreach (var entry in BlockEntities)
            {
                new NbtFile(entry).SaveToStream(writer.BaseStream, NbtCompression.None);
            }
        }
    }
}
