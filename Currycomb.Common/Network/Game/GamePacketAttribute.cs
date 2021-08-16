using System;

namespace Currycomb.Common.Network.Game
{
    public class GamePacketAttribute : Attribute
    {
        public readonly GamePacketId PacketId;
        public GamePacketAttribute(GamePacketId packetId)
        {
            PacketId = packetId;
        }
    }
}
