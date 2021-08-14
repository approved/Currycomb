using System.IO;

namespace Currycomb.Common.Network.Game.Packets
{
    /* 
     * https://dinnerbone.com/blog/2012/01/13/minecraft-plugin-channels-messaging/
     */
    public record PacketCustomPayload(string Channel, byte[] Data) : IGamePacket
    {
        public void Write(BinaryWriter writer)
        {
            if (Channel.Length > 16)
            {
                throw new InvalidDataException($"Length of {nameof(Channel)} cannot exceed 16 characters.");
            }

            if (Data.Length >= short.MaxValue)
            {
                throw new InvalidDataException($"Length of {nameof(Data)} cannot be equal to, or exceed {short.MaxValue} bytes.");
            }

            writer.Write(Channel);
            writer.Write7BitEncodedInt(Data.Length);
            writer.Write(Data);
        }
    }
    public record PacketClientCustomPayload(string Channel, byte[] Data) : PacketCustomPayload(Channel, Data) { }
    public record PacketServerCustomPayload(string Channel, byte[] Data) : PacketCustomPayload(Channel, Data) { }
}
