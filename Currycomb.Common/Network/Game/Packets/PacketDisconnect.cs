using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketDisconnect(string Reason) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write($"{{\"text\":\"{Reason}\"}}");
        }
    }
}
