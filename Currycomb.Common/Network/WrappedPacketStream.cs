using Currycomb.Common.Extensions;
using Microsoft.IO;
using Serilog;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Currycomb.Common.Network
{
    public class WrappedPacketStream : IDisposable
    {
        static ILogger log = Log.ForContext<WrappedPacketStream>();

        // First byte sent for each packet is meta byte
        const byte META_ACK_REQ = 0b00000001; // ACK required
        const byte META_ACK_RES = 0b00000010; // ACK response

        Stream _outputStream;
        RecyclableMemoryStreamManager _msManager;

        ConcurrentDictionary<Guid, TaskCompletionSource<byte>> _blocking = new();
        Channel<WrappedPacketContainer> _queuedPackets = Channel.CreateUnbounded<WrappedPacketContainer>();

        public WrappedPacketStream(Stream stream, RecyclableMemoryStreamManager? msManager = null)
            => (_outputStream, _msManager) = (stream, msManager ?? new());

        private MemoryStream GetMemoryStream() => _msManager.GetStream("WrappedPacketStream");
        private BinaryWriter GetBinaryWriter() => new(GetMemoryStream());

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

                await _outputStream.ReadAsync(metaBuffer, 0, 1);
                byte meta = metaBuffer[0];

                if ((meta & META_ACK_RES) != 0)
                {
                    await _outputStream.ReadAsync(ackBuffer, 0, 16);
                    byte ackVal = (byte)_outputStream.ReadByte();
                    Guid ackGuid = new Guid(ackBuffer);

                    log.Information($"ACK for packet received: {ackGuid}");

                    if (!_blocking.TryRemove(ackGuid, out var tcs) || !tcs.TrySetResult(ackVal))
                    {
                        throw new InvalidOperationException("Failed to handle incoming ACK packet.");
                    }

                    continue;
                }

                log.Information($"Reading incoming wrapped packet");
                Guid? ack = (meta & META_ACK_REQ) != 0 ? await _outputStream.ReadGuidAsync() : null;

                log.Information($"Reading incoming wrapped packet data: {ack}");
                await _queuedPackets.Writer.WriteAsync(new WrappedPacketContainer(ack, await WrappedPacket.ReadAsync(_outputStream)));
                log.Information($"Queued incoming wrapped packet");
            }
        }

        public async Task<byte> SendWaitAsync(WrappedPacket packet)
        {
            log.Information($"Sending awaited packet: {packet}");

            Guid id = Guid.NewGuid();

            await WritePacketAsync(writer =>
            {
                writer.Write(META_ACK_REQ);
                writer.Write(id.ToByteArray());
                packet.WriteTo(writer);
            });

            TaskCompletionSource<byte> tcs = new();
            if (!_blocking.TryAdd(id, tcs))
            {
                throw new InvalidOperationException("Attempted to send packet with a Guid that is already being awaited.");
            }

            log.Information($"Waiting for ACK for packet: {packet}");
            byte b = await tcs.Task;
            log.Information($"Received ACK for packet: {packet}");

            return b;
        }

        public Task SendAsync(WrappedPacket packet)
            => WritePacketAsync(writer =>
            {
                writer.Write((byte)0);
                packet.WriteTo(writer);
            });

        public Task SendAckAsync(Guid ack, byte data = 255)
            => WritePacketAsync(writer =>
            {
                writer.Write(META_ACK_RES);
                writer.Write(ack.ToByteArray());
                writer.Write(data);
            });

        public async Task SendAckAsync(WrappedPacketContainer packet, byte data = 255)
        {
            if (packet.AckGuid is Guid id)
            {
                await SendAckAsync(id, data);
            }
        }

        public async Task<WrappedPacketContainer> ReadAsync(bool autoAck = false, CancellationToken ct = default)
        {
            WrappedPacketContainer packet = await _queuedPackets.Reader.ReadAsync(ct);
            log.Information($"Dequeued packet: {packet}");

            if (!autoAck || !packet.AckGuid.HasValue)
            {
                log.Information($"Returning dequeued packet.");
                return packet;
            }

            await SendAckAsync(packet.AckGuid.Value);
            return packet with { AckGuid = null };
        }

        public async Task<WrappedPacket> ReadAutoAckAsync(CancellationToken ct = default)
        {
            WrappedPacketContainer pktc = await ReadAsync(true, ct);
            WrappedPacket pkt = pktc.Packet;

            await SendAckAsync(pktc);

            log.Information("Returning packet data.");
            return pkt;
        }

        public void Dispose() => _outputStream.Dispose();
    }
}
