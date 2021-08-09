using Currycomb.Common.Extensions;
using Currycomb.Gateway.Network;
using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Currycomb.Gateway.ClientData
{
    public class ClientConnection : IDisposable
    {
        private const int MaximumPacketSize = 0x1FFFFF;

        // Connection to a single MC client
        private readonly NetworkStream _stream;

        // Assign a temporary Guid until we have a "real" one.
        public Guid Id { get; private set; } = Guid.NewGuid();

        // TODO: Wait for client::changed_state broadcast event before continuing with login
        private bool _isInPlayState = false;

        public ClientConnection(NetworkStream stream) => _stream = stream;

        public async Task RunAsync(IncomingPacketRouter incomingPacketDispatcher)
        {
            byte[] bytes = new byte[MaximumPacketSize];
            using MemoryStream memory = new(bytes);

            while (true)
            {
                int length = await _stream.Read7BitEncodedIntAsync();
                if (length > MaximumPacketSize)
                {
                    throw new FormatException($"Length 0x{length:X6} exceeded maximum capacity 0x{MaximumPacketSize:X6}.");
                }

                // Reset the MemoryStream BEFORE reading to the backing buffer, since SetLength zeroes if the buffer grows
                memory.Position = 0;
                memory.SetLength(length);

                for (int i = 0; i < length; i += await _stream.ReadAsync(bytes, i, length - i)) ;

                await incomingPacketDispatcher.DispatchAsync(Id, _isInPlayState, bytes.AsMemory(0, length));
            }
        }

        public void Dispose() => _stream.Dispose();

        public ValueTask SendPacketAsync(ReadOnlyMemory<byte> packetWithoutLength)
            => _stream.WriteAsync(packetWithoutLength);
    }
}
