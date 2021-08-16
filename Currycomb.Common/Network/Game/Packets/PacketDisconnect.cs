using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public abstract record PacketDisconnect(string Reason) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write($"{{\"text\":\"{Reason}\"}}");
        }
    }

    public record PacketDisconnectLogin(string Reason) : PacketDisconnect(Reason) { }
    public record PacketDisconnectPlay(string Reason) : PacketDisconnect(Reason) { }
}
