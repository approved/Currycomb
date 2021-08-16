using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.SetTime)]
    public readonly struct PacketSetTime : IGamePacket
    {
        public readonly long WorldAge;
        public readonly long TimeOfDay;

        public PacketSetTime(long worldAge, long timeOfDay)
        {
            WorldAge = worldAge;
            TimeOfDay = timeOfDay;
        }

        public PacketSetTime(BinaryReader reader)
        {
            WorldAge = reader.ReadInt64();
            TimeOfDay = reader.ReadInt64();
        }

        public void Write(BinaryWriter writer)
        {
            writer.Write(WorldAge);
            writer.Write(TimeOfDay);
        }
    }
}
