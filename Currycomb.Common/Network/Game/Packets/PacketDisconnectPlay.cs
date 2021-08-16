using System.IO;
using Currycomb.Common.Network.Game.Packets.Types;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.DisconnectPlay)]
    public readonly struct PacketDisconnectPlay : IGamePacket
    {
        public readonly Chat Reason;

        public PacketDisconnectPlay(Chat reason)
        {
            Reason = reason;
        }

        public PacketDisconnectPlay(BinaryReader reader)
        {
            Reason = new Chat(reader);
        }

        public void Write(BinaryWriter writer)
        {
            Reason.Write(writer);
        }
    }
}
