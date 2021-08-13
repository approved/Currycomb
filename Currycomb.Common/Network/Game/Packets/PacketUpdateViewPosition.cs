using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketUpdateViewPosition(int ChunkX, int ChunkZ) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(ChunkX);
            writer.Write7BitEncodedInt(ChunkZ);
        }
    }
}
