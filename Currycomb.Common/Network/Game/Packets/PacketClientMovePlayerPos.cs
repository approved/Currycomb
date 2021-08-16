
using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ClientMovePlayerPos)]
    public readonly struct PacketClientMovePlayerPos : IGamePacket
    {
        public readonly bool OnGround;
        public readonly double XPos;
        public readonly double YPos;
        public readonly double ZPos;

        public PacketClientMovePlayerPos(double xPos, double yPos, double zPos, bool onGround)
        {
            XPos = xPos;
            YPos = yPos;
            ZPos = zPos;
            OnGround = onGround;
        }

        public PacketClientMovePlayerPos(BinaryReader reader)
        {
            XPos = reader.ReadDouble();
            YPos = reader.ReadDouble();
            ZPos = reader.ReadDouble();
            OnGround = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(XPos);
            writer.Write(YPos);
            writer.Write(ZPos);
            writer.Write(OnGround);
        }
    }
}
