using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.Response)]
    public readonly struct PacketResponse : IGamePacket
    {
        public readonly String Json;

        public void Write(BinaryWriter stream)
        {
            stream.Write(Json);
        }

        public PacketResponse(String json)
        {
            Json = json;
        }

        public PacketResponse(BinaryReader reader)
        {
            Json = reader.ReadString();
        }
    }
}
