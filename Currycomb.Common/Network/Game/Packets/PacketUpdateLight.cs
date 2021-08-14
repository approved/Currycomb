using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
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
