using System;
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.Handshake)]
    public readonly struct PacketHandshake : IGamePacket
    {
        public readonly uint ProtocolVersion;
        public readonly String ServerAddress;
        public readonly ushort Port;
        public readonly State State;

        public PacketHandshake(BinaryReader stream)
        {
            ProtocolVersion = (uint)stream.Read7BitEncodedInt();
            ServerAddress = stream.ReadString();
            Port = stream.ReadUInt16();
            State = StateExt.FromRaw((uint)stream.Read7BitEncodedInt());
        }

        public PacketHandshake(uint protocolVersion, String serverAddress, ushort port, State state)
        {
            ProtocolVersion = protocolVersion;
            ServerAddress = serverAddress;
            Port = port;
            State = state;
        }

        public void Write(BinaryWriter stream)
        {
            stream.Write7BitEncodedInt((int)ProtocolVersion);
            stream.Write(ServerAddress);
            stream.Write(Port);
            stream.Write7BitEncodedInt((int)State.ToRaw());
        }
    }
}
