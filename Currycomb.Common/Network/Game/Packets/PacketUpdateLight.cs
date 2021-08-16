using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.UpdateLight)]
    public readonly struct PacketUpdateLight : IGamePacket
    {
        public readonly int ChunkX;
        public readonly int ChunkZ;
        public readonly bool TrustEdges;
        public readonly long[] SkyLightMask;
        public readonly long[] BlockLightMask;
        public readonly long[] EmptySkyLightMask;
        public readonly long[] EmptyBlockLightMask;
        public readonly byte[][] SkyLight;
        public readonly byte[][] BlockLight;

        public PacketUpdateLight(int chunkX, int chunkZ, bool trustEdges, long[] skyLightMask, long[] blockLightMask, long[] emptySkyLightMask, long[] emptyBlockLightMask, byte[][] skyLight, byte[][] blockLight)
        {
            ChunkX = chunkX;
            ChunkZ = chunkZ;
            TrustEdges = trustEdges;
            SkyLightMask = skyLightMask;
            BlockLightMask = blockLightMask;
            EmptySkyLightMask = emptySkyLightMask;
            EmptyBlockLightMask = emptyBlockLightMask;
            SkyLight = skyLight;
            BlockLight = blockLight;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(ChunkX);
            writer.Write7BitEncodedInt(ChunkZ);
            writer.Write(TrustEdges);

            writer.Write7BitEncodedInt(SkyLightMask.Length);
            foreach (var entry in SkyLightMask)
                writer.Write(entry);

            writer.Write7BitEncodedInt(BlockLightMask.Length);
            foreach (var entry in BlockLightMask)
                writer.Write(entry);

            writer.Write7BitEncodedInt(EmptySkyLightMask.Length);
            foreach (var entry in EmptySkyLightMask)
                writer.Write(entry);

            writer.Write7BitEncodedInt(EmptyBlockLightMask.Length);
            foreach (var entry in EmptyBlockLightMask)
                writer.Write(entry);

            writer.Write7BitEncodedInt(SkyLight.Length);
            foreach (var entry in SkyLight)
            {
                writer.Write7BitEncodedInt(entry.Length);
                foreach (var value in entry)
                    writer.Write(value);
            }

            writer.Write7BitEncodedInt(BlockLight.Length);
            foreach (var entry in BlockLight)
            {
                writer.Write7BitEncodedInt(entry.Length);
                foreach (var value in entry)
                    writer.Write(value);
            }
        }

        public PacketUpdateLight(BinaryReader reader)
        {
            ChunkX = reader.Read7BitEncodedInt();
            ChunkZ = reader.Read7BitEncodedInt();
            TrustEdges = reader.ReadBoolean();

            SkyLightMask = new long[reader.Read7BitEncodedInt()];
            for (var i = 0; i < SkyLightMask.Length; i++)
                SkyLightMask[i] = reader.ReadInt64();

            BlockLightMask = new long[reader.Read7BitEncodedInt()];
            for (var i = 0; i < BlockLightMask.Length; i++)
                BlockLightMask[i] = reader.ReadInt64();

            EmptySkyLightMask = new long[reader.Read7BitEncodedInt()];
            for (var i = 0; i < EmptySkyLightMask.Length; i++)
                EmptySkyLightMask[i] = reader.ReadInt64();

            EmptyBlockLightMask = new long[reader.Read7BitEncodedInt()];
            for (var i = 0; i < EmptyBlockLightMask.Length; i++)
                EmptyBlockLightMask[i] = reader.ReadInt64();

            SkyLight = new byte[reader.Read7BitEncodedInt()][];
            for (var i = 0; i < SkyLight.Length; i++)
            {
                SkyLight[i] = new byte[reader.Read7BitEncodedInt()];
                for (var j = 0; j < SkyLight[i].Length; j++)
                    SkyLight[i][j] = reader.ReadByte();
            }

            BlockLight = new byte[reader.Read7BitEncodedInt()][];
            for (var i = 0; i < BlockLight.Length; i++)
            {
                BlockLight[i] = new byte[reader.Read7BitEncodedInt()];
                for (var j = 0; j < BlockLight[i].Length; j++)
                    BlockLight[i][j] = reader.ReadByte();
            }
        }
    }
}
