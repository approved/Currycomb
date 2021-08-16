using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.LoginSuccess)]
    public readonly struct PacketLoginSuccess : IGamePacket
    {
        public readonly Guid Uuid;
        public readonly String Username;

        public PacketLoginSuccess(Guid uuid, String username)
        {
            Uuid = uuid;
            Username = username;
        }

        public PacketLoginSuccess(BinaryReader reader)
        {
            Uuid = new Guid(reader.ReadBytes(16));
            Username = reader.ReadString();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(Uuid.ToByteArray());
            writer.Write(Username);
        }
    }
}
