using System.IO;

namespace Currycomb.Common.Network.Meta
{
    public readonly struct MetaPacket
    {
        public static MetaPacket<IMetaPacket> Read(BinaryReader reader)
        {
            var header = MetaPacketHeader.Read(reader);
            var packet = MetaPacketReader.Read(header.PacketId, reader);

            return new()
            {
                Header = header,
                Packet = packet
            };
        }
    }

    public readonly struct MetaPacket<T> where T : IMetaPacket
    {
        public readonly MetaPacketHeader Header { get; init; }
        public readonly T Packet { get; init; }

        public MetaPacket(MetaPacketHeader header, T packet)
        {
            Header = header;
            Packet = packet;
        }

        public void Write(BinaryWriter writer) => Write(Header, Packet, writer);
        public static void Write(MetaPacketHeader header, T packet, BinaryWriter writer)
        {
            header.Write(writer);
            packet.Write(writer);
        }

        // TODO: Use memory stream pool here
        public byte[] ToBytes()
        {
            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);

            Write(bw);

            return ms.ToArray();
        }
    }
}
