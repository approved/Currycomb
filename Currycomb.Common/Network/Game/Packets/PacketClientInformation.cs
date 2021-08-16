using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    [GamePacket(GamePacketId.ClientInformation)]
    public readonly struct PacketClientInformation : IGamePacket
    {
        public readonly string Locale; // TODO: Max length 16
        public readonly byte ViewDistance;
        public readonly int ChatMode;
        public readonly bool EnableChatColors;
        public readonly byte ModelMask;
        public readonly int MainHand;
        public readonly bool EnableTextFiltering;

        public PacketClientInformation(BinaryReader reader)
        {
            Locale = reader.ReadString();
            ViewDistance = reader.ReadByte();
            ChatMode = reader.Read7BitEncodedInt();
            EnableChatColors = reader.ReadBoolean();
            ModelMask = reader.ReadByte();
            MainHand = reader.Read7BitEncodedInt();
            EnableTextFiltering = reader.ReadBoolean();
        }
    }
}
