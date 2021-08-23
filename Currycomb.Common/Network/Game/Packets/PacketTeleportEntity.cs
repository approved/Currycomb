using System.IO;
using Currycomb.Common.Network.Game.Packets.Types;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.TeleportEntity)]
    public readonly struct PacketTeleportEntity : IGamePacket
    {
        public readonly int EntityId;
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly NetAngle Pitch;
        public readonly NetAngle Yaw;
        public readonly bool OnGround;

        public PacketTeleportEntity(int entityId, double x, double y, double z, NetAngle pitch, NetAngle yaw, bool onGround)
        {
            EntityId = entityId;
            X = x;
            Y = y;
            Z = z;
            Pitch = pitch;
            Yaw = yaw;
            OnGround = onGround;
        }

        public PacketTeleportEntity(BinaryReader reader)
        {
            EntityId = reader.Read7BitEncodedInt();
            X = reader.ReadDouble();
            Y = reader.ReadDouble();
            Z = reader.ReadDouble();
            Pitch = new NetAngle(reader);
            Yaw = new NetAngle(reader);
            OnGround = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(EntityId);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            Pitch.Write(writer);
            Yaw.Write(writer);
            writer.Write(OnGround);
        }
    }
}
