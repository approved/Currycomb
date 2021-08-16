using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.PlayerPosition)]
    public readonly struct PacketPlayerPosition : IGamePacket
    {
        public readonly double XPos;
        public readonly double YPos;
        public readonly double ZPos;
        public readonly float Yaw;
        public readonly float Pitch;
        public readonly byte Flags;
        public readonly int TeleportId;
        public readonly bool DoDismountVehicle;

        public PacketPlayerPosition(
            double xPos,
            double yPos,
            double zPos,
            float yaw,
            float pitch,
            byte flags,
            int teleportId,
            bool doDismountVehicle)
        {
            XPos = xPos;
            YPos = yPos;
            ZPos = zPos;
            Yaw = yaw;
            Pitch = pitch;
            Flags = flags;
            TeleportId = teleportId;
            DoDismountVehicle = doDismountVehicle;
        }

        public PacketPlayerPosition(BinaryReader reader)
        {
            XPos = reader.ReadDouble();
            YPos = reader.ReadDouble();
            ZPos = reader.ReadDouble();
            Yaw = reader.ReadSingle();
            Pitch = reader.ReadSingle();
            Flags = reader.ReadByte();
            TeleportId = reader.Read7BitEncodedInt();
            DoDismountVehicle = reader.ReadBoolean();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(XPos);
            writer.Write(YPos);
            writer.Write(ZPos);
            writer.Write(Yaw);
            writer.Write(Pitch);
            writer.Write(Flags);
            writer.Write7BitEncodedInt(TeleportId);
            writer.Write(DoDismountVehicle);
        }
    }
}
