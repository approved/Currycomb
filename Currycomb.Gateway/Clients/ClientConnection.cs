using System;
using System.IO;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Currycomb.Common.Extensions;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Serilog;

namespace Currycomb.Gateway.Clients
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

        private static readonly byte[] AesIsSeeminglyBroken = new byte[15];
        private Aes? _aes;

        public ClientConnection(NetworkStream stream) => _inUseStreamRead = _inUseStreamWrite = _netStream = stream;

        // Assign a temporary Guid until we have a "real" one.
        public Guid Id { get; private set; } = Guid.NewGuid();
        public State State { get; private set; } = State.Handshake;

        public bool ReceiveBroadcasts => State == State.Play;

        public void Dispose() => _netStream.Dispose();

        // TODO(minor): Might want to re-evaluate this.
        public async Task ReadPacketsToChannelAsync(ChannelWriter<(State, WrappedPacket)> writer, CancellationToken ct = default)
        {
            byte[] bytes = new byte[MaximumPacketSize];
            using MemoryStream memory = new(bytes);

            // TODO: Use the cancellation token properly
            while (!ct.IsCancellationRequested)
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

                await writer.WriteAsync((State, new WrappedPacket(Id, memory.ToArray())));
            }
        }

        public async Task SendPacketAsync(ReadOnlyMemory<byte> packet)
        {
            try
            {
                await _inUseStreamWrite.WriteAsync(packet);

                if (_encryptStreamWrite != null)
                {
                    // This is done due to .NET AES CFB8 seemingly not respecting FeedbackSize,
                    // instead requiring a multiple of 16 bytes to be written per "block".
                    // Until a full block of 16 bytes has been written to the stream, the last
                    // block will have its "tail" stuck in limbo waiting for another packet to
                    // finish the "block". This results in a lot of weird behaviour since packets
                    // are usually not sent in their entirety until another packet comes along and
                    // "finishes" them by writing past the seemingly arbitrary 16-byte boundary.
                    await _inUseStreamWrite.WriteAsync(AesIsSeeminglyBroken);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error writing packet to client.");
            }
        }

        public void SetAesKey(byte[] key)
        {
            Log.Debug("Enabling AES for ClientConnection {@id}.", Id);

            _aes = Aes.Create();
            _aes.KeySize = 128;
            _aes.BlockSize = 128;

            _aes.Mode = CipherMode.CFB;
            _aes.Padding = PaddingMode.None;
            _aes.FeedbackSize = 8;

            _aes.Key = key;
            _aes.IV = key;

            _encryptStreamRead = new CryptoStream(_netStream, _aes.CreateDecryptor(), CryptoStreamMode.Read);
            _encryptStreamWrite = new CryptoStream(_netStream, _aes.CreateEncryptor(), CryptoStreamMode.Write);

            RefreshInUseStream();
        }

        public void SetState(State state)
        {
            Log.Debug("Setting State for ClientConnection {@id}.", Id);

            State = state;
        }

        private void RefreshInUseStream()
        {
            _inUseStreamRead = _encryptStreamRead ?? _netStream;
            _inUseStreamWrite = _encryptStreamWrite ?? _netStream;
        }
    }
}
