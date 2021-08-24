using Currycomb.Common.Network.Game.Packets.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.SpawnMob)]
    public readonly struct PacketSpawnMob : IGamePacket
    {
        public readonly int EntityId;
        public readonly Guid ObjectUuid;
        public readonly EntityType Type;
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly NetAngle Pitch;
        public readonly NetAngle Yaw;
        public readonly NetAngle HeadRotation;
        public readonly short VelocityX;
        public readonly short VelocityY;
        public readonly short VelocityZ;

        public PacketSpawnMob(int entityId, Guid objectUuid, EntityType type, double x, double y, double z, NetAngle pitch, NetAngle yaw, NetAngle headRotation, short velocityX, short velocityY, short velocityZ)
        {
            EntityId = entityId;
            ObjectUuid = objectUuid;
            Type = type;
            X = x;
            Y = y;
            Z = z;
            Pitch = pitch;
            Yaw = yaw;
            HeadRotation = headRotation;
            VelocityX = velocityX;
            VelocityY = velocityY;
            VelocityZ = velocityZ;
        }

        public PacketSpawnMob(BinaryReader reader)
        {
            EntityId = reader.Read7BitEncodedInt();
            ObjectUuid = new Guid(reader.ReadBytes(16));
            Type = (EntityType)reader.Read7BitEncodedInt();
            X = reader.ReadDouble();
            Y = reader.ReadDouble();
            Z = reader.ReadDouble();
            Pitch = new NetAngle(reader);
            Yaw = new NetAngle(reader);
            HeadRotation = new NetAngle(reader);
            VelocityX = reader.ReadInt16();
            VelocityY = reader.ReadInt16();
            VelocityZ = reader.ReadInt16();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write7BitEncodedInt(EntityId);
            writer.Write(ObjectUuid.ToByteArray());
            writer.Write7BitEncodedInt((int)Type);
            writer.Write(X);
            writer.Write(Y);
            writer.Write(Z);
            Pitch.Write(writer);
            Yaw.Write(writer);
            HeadRotation.Write(writer);
            writer.Write(VelocityX);
            writer.Write(VelocityY);
            writer.Write(VelocityZ);
        }
    }
}
