using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ChunkCacheCenter)]
    public readonly struct PacketChunkCacheCenter : IGamePacket
    {
        public readonly int ChunkX;
        public readonly int ChunkZ;

        public PacketChunkCacheCenter(BinaryReader reader)
        {
            ChunkX = reader.Read7BitEncodedInt();
            ChunkZ = reader.Read7BitEncodedInt();
        }

        public PacketChunkCacheCenter(int chunkX, int chunkZ)
        {
            ChunkX = chunkX;
            ChunkZ = chunkZ;
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(ChunkX);
            writer.Write7BitEncodedInt(ChunkZ);
        }
    }
}
