using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace Currycomb.Common.Extensions
{
    public static class StreamExtensions
    {
        #region Write
        public static async Task Write7BitEncodedUIntAsync(this Stream stream, uint value) => await Write7BitEncodedIntAsync(stream, (int)value);
        public static async Task Write7BitEncodedIntAsync(this Stream stream, int value)
        {
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value;   // support negative numbers
            byte[] b = new byte[1];
            while (v >= 0x80)
            {
                b[0] = (byte)(v | 0x80);
                await stream.WriteAsync(b);
                v >>= 7;
            }
            b[0] = (byte)v;
            await stream.WriteAsync(b);
        }

        public static void Write7BitEncodedUInt(this Stream stream, uint value) => Write7BitEncodedInt(stream, (int)value);
        public static void Write7BitEncodedInt(this Stream stream, int value)
        {
            // Write out an int 7 bits at a time.  The high bit of the byte,
            // when on, tells reader to continue reading more bytes.
            uint v = (uint)value;   // support negative numbers
            while (v >= 0x80)
            {
                stream.WriteByte((byte)(v | 0x80));
                v >>= 7;
            }
            stream.WriteByte((byte)v);
        }

        public static async Task WriteAsync(this Stream stream, string str, Encoding? encoding = null)
        {
            byte[] bytes = (encoding ?? Encoding.UTF8).GetBytes(str);

            Log.Error($"Writing string: {bytes.Length}");

            await stream.Write7BitEncodedIntAsync(bytes.Length);
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public static Task WriteAsync(this Stream stream, Guid guid)
            => stream.WriteAsync(guid.ToByteArray(), 0, 16);

        public static Task WriteAsync(this Stream stream, long value)
        {
            byte[] buffer = new byte[8];
            buffer[7] = (byte)value;
            buffer[6] = (byte)(value >> 8);
            buffer[5] = (byte)(value >> 16);
            buffer[4] = (byte)(value >> 24);
            buffer[3] = (byte)(value >> 32);
            buffer[2] = (byte)(value >> 40);
            buffer[1] = (byte)(value >> 48);
            buffer[0] = (byte)(value >> 56);
            return stream.WriteAsync(buffer, 0, 8);
        }
        #endregion

        #region Read
        public static async Task<uint> Read7BitEncodedUIntAsync(this Stream stream) => (uint)(await Read7BitEncodedIntAsync(stream));
        public static async Task<int> Read7BitEncodedIntAsync(this Stream stream)
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

                // ReadAsync handles end of stream cases for us.
                if (await stream.ReadAsync(byteBuffer, 0, 1) == 0)
                {
                    throw new ObjectDisposedException("Cannot access a closed Stream.");
                }

                b = byteBuffer[0];
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }

        public static uint Read7BitEncodedUInt(this Stream stream) => (uint)Read7BitEncodedInt(stream);
        public static int Read7BitEncodedInt(this Stream stream)
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

                // Read handles end of stream cases for us.
                if (stream.Read(byteBuffer, 0, 1) == 0)
                {
                    throw new ObjectDisposedException("Cannot access a closed Stream.");
                }

                b = byteBuffer[0];
                count |= (b & 0x7F) << shift;
                shift += 7;
            } while ((b & 0x80) != 0);
            return count;
        }

        public static async Task<ushort> ReadUShortAsync(this Stream stream) => (ushort)(await ReadShortAsync(stream));
        public static async Task<short> ReadShortAsync(this Stream stream)
        {
            byte[] buffer = new byte[2];

            if (await stream.ReadAsync(buffer, 0, 2) == 0)
            {
                throw new ObjectDisposedException("Cannot access a closed Stream.");
            }

            return (short)(buffer[0] << 8 | buffer[1]);
        }

        public static async Task<string> ReadStringAsync(this Stream stream, ushort? maxLength = null, Encoding? encoding = null)
        {
            int length = await stream.Read7BitEncodedIntAsync();
            if (maxLength.HasValue && length > maxLength.Value)
            {
                throw new InvalidDataException($"Read length {length} exceeded maximum length {maxLength}!");
            }

            byte[] buffer = new byte[length];
            if (await stream.ReadAsync(buffer, 0, length) == 0)
            {
                throw new ObjectDisposedException("Cannot access a closed Stream.");
            }

            return (encoding ?? Encoding.UTF8).GetString(buffer);
        }

        public static async Task<byte[]> ReadBytesAsync(this Stream stream, int length)
        {
            byte[] buffer = new byte[length];
            if (await stream.ReadAsync(buffer, 0, length) == 0)
                throw new ObjectDisposedException("Cannot read from closed Stream.");
            return buffer;
        }

        public static async Task<Guid> ReadGuidAsync(this Stream stream)
            => new Guid(await ReadBytesAsync(stream, 16));

        public static async Task<long> ReadLongAsync(this Stream stream)
        {
            byte[] buffer = await ReadBytesAsync(stream, 8);

            long value = 0;
            value |= (long)buffer[7];
            value |= (long)buffer[6] << 8;
            value |= (long)buffer[5] << 16;
            value |= (long)buffer[4] << 24;
            value |= (long)buffer[3] << 32;
            value |= (long)buffer[2] << 40;
            value |= (long)buffer[1] << 48;
            value |= (long)buffer[0] << 56;
            return value;
        }
        #endregion
    }
}
