using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    public record PacketPlayerPosition(double XPos, double YPos, double ZPos, float Yaw, float Pitch, int TeleportId, bool DoDismountVehicle) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            writer.Write(XPos);
            writer.Write(YPos);
            writer.Write(ZPos);
            writer.Write(Yaw);
            writer.Write(Pitch);
            writer.Write(TeleportId);
            writer.Write(DoDismountVehicle);
        }
    }
}
