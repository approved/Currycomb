using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.LoginStart)]
    public readonly struct PacketLoginStart : IGamePacket
    {
        public readonly String Username;

        public PacketLoginStart(BinaryReader reader)
        {
            Username = reader.ReadString();
        }
    }
}
