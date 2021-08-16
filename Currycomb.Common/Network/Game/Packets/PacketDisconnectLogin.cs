using System.IO;
using Currycomb.Common.Network.Game.Packets.Types;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.DisconnectLogin)]
    public readonly struct PacketDisconnectLogin : IGamePacket
    {
        public readonly Chat Reason;

        public PacketDisconnectLogin(Chat reason)
        {
            Reason = reason;
        }

        public PacketDisconnectLogin(BinaryReader reader)
        {
            Reason = new Chat(reader);
        }

        public void Write(BinaryWriter writer)
        {
            Reason.Write(writer);
        }
    }
}
