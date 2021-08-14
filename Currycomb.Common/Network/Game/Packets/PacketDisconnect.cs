using System.IO;
using fNbt;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketDisconnect(string Reason) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write($"{{\"text\":\"{Reason}\"}}");
        }
    }

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

    public record PacketUpdateLight(int ChunkX, int ChunkZ, bool TrustEdges, long[] SkyLightMask, long[] BlockLightMask, long[] EmptySkyLightMask, long[] EmptyBlockLightMask, byte[][] SkyLight, byte[][] BlockLight) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(ChunkX);
            writer.Write7BitEncodedInt(ChunkZ);
            writer.Write(TrustEdges);
            writer.Write7BitEncodedInt(SkyLightMask.Length);
            foreach (var entry in SkyLightMask)
            {
                writer.Write(entry);
            }
            writer.Write7BitEncodedInt(BlockLightMask.Length);
            foreach (var entry in BlockLightMask)
            {
                writer.Write(entry);
            }
            writer.Write7BitEncodedInt(EmptySkyLightMask.Length);
            foreach (var entry in EmptySkyLightMask)
            {
                writer.Write(entry);
            }
            writer.Write7BitEncodedInt(EmptyBlockLightMask.Length);
            foreach (var entry in EmptyBlockLightMask)
            {
                writer.Write(entry);
            }
            writer.Write7BitEncodedInt(SkyLight.Length);
            foreach (var entry in SkyLight)
            {
                writer.Write7BitEncodedInt(entry.Length);
                foreach (var value in entry)
                {
                    writer.Write(value);
                }
            }
            writer.Write7BitEncodedInt(BlockLight.Length);
            foreach (var entry in BlockLight)
            {
                writer.Write7BitEncodedInt(entry.Length);
                foreach (var value in entry)
                {
                    writer.Write(value);
                }
            }
        }
    }
}
