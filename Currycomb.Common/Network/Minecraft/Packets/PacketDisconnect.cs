using System.IO;

namespace Currycomb.Common.Network.Minecraft.Packets
{
    public record PacketDisconnect(string Reason) : IPacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write($"{{\"text\":\"{Reason}\"}}");
        }
    }
}
