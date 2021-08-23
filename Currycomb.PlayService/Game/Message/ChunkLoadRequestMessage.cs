namespace Currycomb.PlayService.Game.Message
{
    public readonly struct ChunkLoadRequestMessage
    {
        public readonly int ChunkX;
        public readonly int ChunkZ;

        public ChunkLoadRequestMessage(int chunkX, int chunkZ)
        {
            ChunkX = chunkX;
            ChunkZ = chunkZ;
        }
    }
}