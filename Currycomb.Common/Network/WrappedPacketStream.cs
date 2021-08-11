using Currycomb.Common.Extensions;
using Microsoft.IO;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Currycomb.Common.Network
{
    public class WrappedPacketStream : IDisposable
    {
        private readonly static ILogger log = Log.ForContext<WrappedPacketStream>();

        // First byte sent for each packet is meta byte
        const byte META_ACK_REQ = 0b00000001; // ACK required
        const byte META_ACK_RES = 0b00000010; // ACK response
        const byte META_INT_PKT = 0b00000100; // Paylod is Internal/Meta Packet - Not Minecraft

        readonly Stream _outputStream;
        readonly RecyclableMemoryStreamManager _msManager;
        readonly ConcurrentDictionary<Guid, TaskCompletionSource<byte>> _blocking = new();
        readonly Channel<WrappedPacketContainer> _queuedPackets = Channel.CreateUnbounded<WrappedPacketContainer>();

        public WrappedPacketStream(Stream stream, RecyclableMemoryStreamManager? msManager = null)
            => (_outputStream, _msManager) = (stream, msManager ?? new());

        private MemoryStream GetMemoryStream()
            => _msManager.GetStream("WrappedPacketStream");

        private BinaryWriter GetBinaryWriter()
            => new(GetMemoryStream());

        private async Task WritePacketAsync(Action<BinaryWriter> action)
        {
            using var writer = GetBinaryWriter();
            action(writer);
            writer.BaseStream.Position = 0;
            await writer.BaseStream.CopyToAsync(_outputStream);
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            byte[] ackBuffer = new byte[16];
            byte[] metaBuffer = new byte[1];

            while (!ct.IsCancellationRequested)
            {
                // TODO: Handle errors

                log.Information("Waiting for incoming packet");

                await _outputStream.ReadAsync(metaBuffer.AsMemory(0, 1), ct);
                byte meta = metaBuffer[0];

                if ((meta & META_ACK_RES) != 0)
                {
                    await _outputStream.ReadAsync(ackBuffer.AsMemory(0, 16), ct);
                    byte ackVal = (byte)_outputStream.ReadByte();
                    Guid ackGuid = new Guid(ackBuffer);

                    log.Information($"ACK for packet received: {ackGuid}");

                    if (!_blocking.TryRemove(ackGuid, out var tcs) || !tcs.TrySetResult(ackVal))
                    {
                        throw new InvalidOperationException("Failed to handle incoming ACK packet.");
                    }

                    continue;
                }

                Guid? acknowledgement = (meta & META_ACK_REQ) != 0 ? await _outputStream.ReadGuidAsync() : null;
                bool isInternalPacket = (meta & META_INT_PKT) != 0;

                log.Information("Reading incoming wrapped packet {ack}", acknowledgement);
                await _queuedPackets.Writer.WriteAsync(new WrappedPacketContainer(acknowledgement, isInternalPacket, await WrappedPacket.ReadAsync(_outputStream)), ct);
                log.Information($"Queued incoming wrapped packet");
            }
        }

        public async Task<byte> SendWaitAsync(WrappedPacket packet, bool isMeta)
        {
            var meta = (byte)(META_ACK_REQ | (isMeta ? META_INT_PKT : 0));
            Guid ack = Guid.NewGuid();

            log.Information("Sending awaited packet: {@ack}", ack);

            TaskCompletionSource<byte> tcs = new();
            if (!_blocking.TryAdd(ack, tcs))
            {
                throw new InvalidOperationException("Attempted to send packet with a Guid that is already being awaited.");
            }

            try
            {
                log.Information("Sending and waiting for ACK for packet: {@ack}", ack);
                await WritePacketAsync(writer =>
                {
                    writer.Write(meta);
                    writer.Write(ack.ToByteArray());
                    packet.WriteTo(writer);
                });
            }
            catch
            {
                _blocking.TryRemove(ack, out _);
                throw;
            }

            return await tcs.Task;
        }

        public Task SendAsync(WrappedPacket packet, bool isMeta)
            => WritePacketAsync(writer =>
            {
                writer.Write((byte)(isMeta ? META_INT_PKT : 0));
                packet.WriteTo(writer);
            });

        public Task SendAckAsync(Guid ack, byte data = 255)
            => WritePacketAsync(writer =>
            {
                log.Debug("Sending ACK for packet {@id}", ack);

                writer.Write(META_ACK_RES);
                writer.Write(ack.ToByteArray());
                writer.Write(data);
            });

        public async Task SendAckAsync(WrappedPacketContainer pktc, byte data = 255)
        {
            if (pktc.AckGuid is Guid id)
            {
                await SendAckAsync(id, data);
            }
        }

        public async Task<WrappedPacketContainer> ReadAsync(bool autoAck = false, CancellationToken ct = default)
        {
            WrappedPacketContainer pktc = await _queuedPackets.Reader.ReadAsync(ct);
            log.Information("Received packet {guid}", pktc.AckGuid);

            if (!autoAck || !pktc.AckGuid.HasValue)
            {
                return pktc;
            }

            await SendAckAsync(pktc.AckGuid.Value);
            return pktc with { AckGuid = null };
        }

        public void Dispose()
        {
            _outputStream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
