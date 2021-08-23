using System;
using System.IO;
using Currycomb.Common.Network.Game.Packets.Types;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.SpawnEntity)]
    public readonly struct PacketSpawnEntity : IGamePacket
    {
        public readonly int EntityId;
        public readonly Guid ObjectUuid;
        public readonly EntityType Type;
        public readonly double X;
        public readonly double Y;
        public readonly double Z;
        public readonly NetAngle Pitch;
        public readonly NetAngle Yaw;
        public readonly int Data;
        public readonly short VelocityX;
        public readonly short VelocityY;
        public readonly short VelocityZ;

        public PacketSpawnEntity(
            int entityId,
            Guid objectUuid,
            EntityType type,
            double x,
            double y,
            double z,
            NetAngle pitch,
            NetAngle yaw,
            int data,
            short velocityX,
            short velocityY,
            short velocityZ)
        {
            EntityId = entityId;
            ObjectUuid = objectUuid;
            Type = type;
            X = x;
            Y = y;
            Z = z;
            Pitch = pitch;
            Yaw = yaw;
            Data = data;
            VelocityX = velocityX;
            VelocityY = velocityY;
            VelocityZ = velocityZ;
        }

        public PacketSpawnEntity(BinaryReader reader)
        {
            EntityId = reader.Read7BitEncodedInt();
            ObjectUuid = new Guid(reader.ReadBytes(16));
            Type = (EntityType)reader.Read7BitEncodedInt();
            X = reader.ReadDouble();
            Y = reader.ReadDouble();
            Z = reader.ReadDouble();
            Pitch = new NetAngle(reader);
            Yaw = new NetAngle(reader);
            Data = reader.ReadInt32();
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
            writer.Write(Data);
            writer.Write(VelocityX);
            writer.Write(VelocityY);
            writer.Write(VelocityZ);
        }
    }
}
