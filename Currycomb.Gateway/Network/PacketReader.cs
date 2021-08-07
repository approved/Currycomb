using System.IO;
using System.Text;

namespace Currycomb.Gateway.Network
{
    public class PacketReader : BinaryReader
    {
        public PacketReader(Stream stream) : base(stream) { }

        public PacketReader(Stream stream, Encoding encoding, bool leaveOpen = false) : base(stream, encoding, leaveOpen) { }

        public string ReadSizedString(int capacity)
        {
            int length = Read7BitEncodedInt();
            if (length > capacity)
            {
                throw new InvalidDataException($"Length ({length}) exceeded maximum capacity ({capacity})!");
            }

            if (length == 0)
            {
                return string.Empty;
            }

            return new(ReadChars(length));
        }

        public ushort ReadPort()
        {
            byte high = ReadByte();
            byte low = ReadByte();
            return (ushort)(high << 8 | low);
        }
    }
}
