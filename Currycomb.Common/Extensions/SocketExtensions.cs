using System;
using System.Net.Sockets;

namespace Currycomb.Common.Extensions
{
    // Source: https://stackoverflow.com/a/36733060/6713695

    public static class SocketExtensions
    {
        private const int BitsPerByte = 8;
        private const int BytesPerLong = 4; // 32 / 8

        /// <summary>
        /// Does an active check for whether or not a socket is connected.
        /// This is not a thread-safe operation.
        /// </summary>
        /// <param name="socket">The socket to check.</param>
        /// <returns>True if the socket is connected, false otherwise.</returns>
        public static bool IsConnected(this Socket socket)
        {
            try
            {
                return !(socket.Poll(1000, SelectMode.SelectRead) && socket.Available == 0);
            }
            catch (SocketException)
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the keep-alive interval for the socket.
        /// </summary>
        /// <param name="socket">The socket.</param>
        /// <param name="time">Milliseconds between two keep alive "pings".</param>
        /// <param name="interval">Milliseconds between two keep alive "pings" when first one fails.</param>
        /// <returns>If the keep alive infos were succefully modified.</returns>
        public static bool SetKeepAlive(this Socket socket, ulong time, ulong interval)
        {
            try
            {
                // Array to hold input values.
                var input = new[]
                {
                    (time == 0 || interval == 0) ? 0UL : 1UL, // on or off
                    time,
                    interval
                };

                // Pack input into byte struct.
                byte[] inValue = new byte[3 * BytesPerLong];
                for (int i = 0; i < input.Length; i++)
                {
                    inValue[i * BytesPerLong + 3] = (byte)(input[i] >> ((BytesPerLong - 1) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 2] = (byte)(input[i] >> ((BytesPerLong - 2) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 1] = (byte)(input[i] >> ((BytesPerLong - 3) * BitsPerByte) & 0xff);
                    inValue[i * BytesPerLong + 0] = (byte)(input[i] >> ((BytesPerLong - 4) * BitsPerByte) & 0xff);
                }

                // Create bytestruct for result (bytes pending on server socket).
                byte[] outValue = BitConverter.GetBytes(0);

                // Write SIO_VALS to Socket IOControl.
                socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                socket.IOControl(IOControlCode.KeepAliveValues, inValue, outValue);
            }
            catch (SocketException)
            {
                return false;
            }

            return true;
        }
    }
}