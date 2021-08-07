using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Currycomb.Gateway.Extensions
{
    public static class NetworkStreamExtensions
    {
        public static async Task<int> Read7BitEncodedIntAsync(this NetworkStream networkStream)
        {
            // Read out an Int32 7 bits at a time.  The high bit
            // of the byte when on means to continue reading more bytes.
            int count = 0;
            int shift = 0;
            byte b;
            byte[] byteBuffer = new byte[1];
            do
            {
                // Check for a corrupted stream.  Read a max of 5 bytes.
                // In a future version, add a DataFormatException.
                if (shift == 5 * 7)  // 5 bytes max per Int32, shift += 7
                    throw new FormatException("Format_Bad7BitInt32");

                // ReadByte handles end of stream cases for us.
                if (await networkStream.ReadAsync(byteBuffer, 0, 1) == 0)
                {
                    throw new ObjectDisposedException("Cannot access a closed Stream.");
                }

                b = byteBuffer[0];
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }
    }
}
