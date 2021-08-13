using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketTimeUpdate(long WorldAge, long TimeOfDay) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(WorldAge);
            writer.Write(TimeOfDay);
        }
    }
}
