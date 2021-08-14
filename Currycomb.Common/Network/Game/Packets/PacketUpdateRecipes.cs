using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public class PacketUpdateRecipes : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(0);
        }
    }
}
