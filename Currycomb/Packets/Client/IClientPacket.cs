namespace Currycomb.Packets.Client
{
    public interface IClientPacket
    {
        /// <summary>
        /// A callback function required by all client packets to handle dispatching the relevant server packets.
        /// </summary>
        void ProcessInformation(PacketReader packetReader);
    }
}
