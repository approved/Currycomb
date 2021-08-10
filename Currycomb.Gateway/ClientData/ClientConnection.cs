using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;
using Currycomb.Common.Network.Game;
using Currycomb.Gateway.Network;
using Serilog;

namespace Currycomb.Gateway.ClientData
{
    public class ClientConnection : IDisposable
    {
        private const int MaximumPacketSize = 0x1FFFFF;

        // Connection to a single MC client
        private readonly NetworkStream _netStream;

        private Stream? _encryptStreamRead;
        private Stream? _encryptStreamWrite;

        private Stream _inUseStreamRead;
        private Stream _inUseStreamWrite;

        private bool _isInPlayState = false;

        public ClientConnection(NetworkStream stream) => _inUseStreamRead = _inUseStreamWrite = _netStream = stream;

        // Assign a temporary Guid until we have a "real" one.
        public Guid Id { get; private set; } = Guid.NewGuid();
        public State State { get; private set; } = State.Handshake;

        public void Dispose() => _netStream.Dispose();

        public async Task RunAsync(PacketServiceRouter incomingPacketDispatcher)
        {
            byte[] bytes = new byte[MaximumPacketSize];
            using MemoryStream memory = new(bytes);

            while (true)
            {
                int length = await _inUseStreamRead.Read7BitEncodedIntAsync();
                if (length > MaximumPacketSize)
                {
                    throw new FormatException($"Length 0x{length:X6} exceeded maximum capacity 0x{MaximumPacketSize:X6}.");
                }

                // Reset the MemoryStream BEFORE reading to the backing buffer, since SetLength zeroes if the buffer grows
                memory.Position = 0;
                memory.SetLength(length);

                for (int i = 0; i < length; i += await _inUseStreamRead.ReadAsync(bytes, i, length - i)) ;

                await incomingPacketDispatcher.DispatchAsync(Id, _isInPlayState, bytes.AsMemory(0, length));
            }
        }

        public ValueTask SendPacketAsync(ReadOnlyMemory<byte> packetWithoutLength)
            => _inUseStreamWrite.WriteAsync(packetWithoutLength);

        public void SetAesKey(byte[] key)
        {
            Log.Debug("Enabling AES for ClientConnection {@id}.", Id);

            _encryptStreamRead = new CryptoStream(_netStream, new AesCryptoServiceProvider().CreateDecryptor(key, key), CryptoStreamMode.Read);
            _encryptStreamWrite = new CryptoStream(_netStream, new AesCryptoServiceProvider().CreateEncryptor(key, key), CryptoStreamMode.Write);

            RefreshInUseStream();
        }

        public void SetState(State state)
        {
            Log.Debug("Setting State for ClientConnection {@id}.", Id);

            State = state;
            _isInPlayState = state == State.Play;
        }

        private void RefreshInUseStream()
        {
            _inUseStreamRead = _encryptStreamRead ?? _netStream;
            _inUseStreamWrite = _encryptStreamWrite ?? _netStream;
        }
    }
}
