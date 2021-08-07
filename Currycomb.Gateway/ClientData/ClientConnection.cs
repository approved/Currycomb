using Currycomb.Gateway.Extensions;
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

        private bool _disposedValue;
        private readonly NetworkStream _stream;

        // Assign a temporary Guid until we have a "real" one.
        // TODO: Let this be reassigned by the service handling auth.
        private Guid _id = Guid.NewGuid();

        private bool _isInPlayState = false;

        public ClientConnection(NetworkStream stream)
        {
            _stream = stream;
        }

        public async Task RunAsync(IncomingPacketDispatcher incomingPacketDispatcher)
        {
            if (_disposedValue)
            {
                throw new ObjectDisposedException(nameof(ClientConnection));
            }

            byte[] bytes = new byte[MaximumPacketSize];
            using MemoryStream memory = new(bytes);
            using PacketReader packetReader = new(memory);

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

                incomingPacketDispatcher.Dispatch(_id, _isInPlayState, packetReader);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _stream.Dispose();
                }

                // TODO: set large fields to null
                _disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
