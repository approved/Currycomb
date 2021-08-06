using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Packets
{
    public class PacketReader : BinaryReader
    {
        public PacketReader(Stream stream): base (stream) { }

        public PacketReader(Stream stream, Encoding encoding, bool leaveOpen = false): base (stream, encoding, leaveOpen) { }

        public string ReadSizedString(int capacity)
        {
            string str = ReadString();
            if (str.Length == 0)
            {
                return string.Empty;
            }
            else if (str.Length <= capacity)
            {
                return str;
            }

            throw new InvalidDataException($"Length ({str.Length}) exceeded maximum capacity ({capacity})!");
        }

        public ushort ReadPort()
        {
            byte high = ReadByte();
            byte low = ReadByte();
            return (ushort) (high << 8 | low);
        }
    }
}
