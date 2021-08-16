using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    // https://dinnerbone.com/blog/2012/01/13/minecraft-plugin-channels-messaging/
    [GamePacket(GamePacketId.ServerCustomPayload)]
    public readonly struct PacketServerCustomPayload : IGamePacket
    {
        public readonly string Channel;
        public readonly byte[] Data;

        public PacketServerCustomPayload(string channel, byte[] data)
        {
            Channel = channel;
            Data = data;
        }

        public PacketServerCustomPayload(BinaryReader reader)
        {
            Channel = reader.ReadString();
            Data = reader.ReadBytes(reader.ReadInt32());
        }

        public void Write(BinaryWriter writer)
        {
            if (Channel.Length > 16)
                throw new InvalidDataException($"Length of {nameof(Channel)} cannot exceed 16 characters.");

            if (Data.Length >= short.MaxValue)
                throw new InvalidDataException($"Length of {nameof(Data)} cannot be equal to, or exceed {short.MaxValue} bytes.");

            writer.Write(Channel);
            writer.Write7BitEncodedInt(Data.Length);
            writer.Write(Data);
        }
    }
}
