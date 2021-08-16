using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ClientMovePlayer)]
    public readonly struct PacketClientMovePlayer : IGamePacket
    {
        public readonly double XPos;
        public readonly double YPos;
        public readonly double ZPos;
        public readonly float Yaw;
        public readonly float Pitch;
        public readonly bool OnGround;

        public PacketClientMovePlayer(double xPos, double yPos, double zPos, float yaw, float pitch, bool onGround)
        {
            XPos = xPos;
            YPos = yPos;
            ZPos = zPos;
            Yaw = yaw;
            Pitch = pitch;
            OnGround = onGround;
        }

        public PacketClientMovePlayer(BinaryReader reader)
        {
            XPos = reader.ReadDouble();
            YPos = reader.ReadDouble();
            ZPos = reader.ReadDouble();
            Yaw = reader.ReadSingle();
            Pitch = reader.ReadSingle();
            OnGround = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(XPos);
            writer.Write(YPos);
            writer.Write(ZPos);
            writer.Write(Yaw);
            writer.Write(Pitch);
            writer.Write(OnGround);
        }
    }
}
