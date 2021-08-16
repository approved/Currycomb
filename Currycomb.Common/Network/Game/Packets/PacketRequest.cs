using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.Request)]
    public readonly struct PacketRequest : IGamePacket
    {
        public PacketRequest(BinaryReader reader) { }
        public void Write(BinaryWriter writer) { }
    }
}
