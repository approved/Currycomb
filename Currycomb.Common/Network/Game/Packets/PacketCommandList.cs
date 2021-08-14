using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public class PacketCommandList : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(1);
            writer.Write((byte)0);
            writer.Write7BitEncodedInt(0);
            writer.Write7BitEncodedInt(0);
        }
    }
}
