using Currycomb.Common.Extensions;
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
        // First byte sent for each packet is meta byte
        const byte META_ACK_REQ = 0b00000001; // ACK required
        const byte META_ACK_RES = 0b00000010; // ACK response

        Stream _stream;

        ConcurrentDictionary<Guid, TaskCompletionSource<byte>> _blocking = new();
        Channel<WrappedPacketContainer> _queuedPackets = Channel.CreateUnbounded<WrappedPacketContainer>();

        public WrappedPacketStream(Stream stream) => _stream = stream;

        public async Task RunAsync(CancellationToken ct = default)
        {
            byte[] ackBuffer = new byte[16];
            byte[] metaBuffer = new byte[1];

            while (!ct.IsCancellationRequested)
            {
                // TODO: Handle errors

                Log.Information("Waiting for incoming packet");

                await _stream.ReadAsync(metaBuffer, 0, 1);
                byte meta = metaBuffer[0];

                if ((meta & META_ACK_RES) != 0)
                {
                    await _stream.ReadAsync(ackBuffer, 0, 16);
                    byte ackVal = (byte)_stream.ReadByte();
                    Guid ackGuid = new Guid(ackBuffer);

                    Log.Information($"ACK for packet received: {ackGuid}");

                    if (!_blocking.TryRemove(ackGuid, out var tcs) || !tcs.TrySetResult(ackVal))
                    {
                        throw new InvalidOperationException("Failed to handle incoming ACK packet.");
                    }

                    continue;
                }

                Log.Information($"Reading incoming wrapped packet");
                Guid? ack = (meta & META_ACK_REQ) != 0 ? await _stream.ReadGuidAsync() : null;

                Log.Information($"Reading incoming wrapped packet data: {ack}");
                await _queuedPackets.Writer.WriteAsync(new WrappedPacketContainer(ack, await WrappedPacket.ReadAsync(_stream)));
                Log.Information($"Queued incoming wrapped packet");
            }
        }

        public async Task<byte> SendWaitAsync(WrappedPacket packet)
        {
            Log.Information($"Sending awaited packet: {packet}");
            _stream.WriteByte(META_ACK_REQ);

            Guid id = Guid.NewGuid();

            await _stream.WriteAsync(id.ToByteArray());
            await packet.WriteAsync(_stream);

            TaskCompletionSource<byte> tcs = new();
            if (!_blocking.TryAdd(id, tcs))
            {
                throw new InvalidOperationException("Attempted to send packet with a Guid that is already being awaited.");
            }

            Log.Information($"Waiting for ACK for packet: {packet}");
            byte b = await tcs.Task;
            Log.Information($"Received ACK for packet: {packet}");

            return b;
        }

        public Task SendAsync(WrappedPacket packet)
        {
            _stream.WriteByte(0);
            return packet.WriteAsync(_stream);
        }

        public async Task SendAckAsync(Guid ack, byte data = 255)
        {
            Log.Information($"Writing ACK for {ack}");

            _stream.WriteByte(META_ACK_RES);
            await _stream.WriteAsync(ack);
            _stream.WriteByte(data);
        }

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
            Log.Information($"Dequeued packet: {packet}");

            if (!autoAck || !packet.AckGuid.HasValue)
            {
                Log.Information($"Returning dequeued packet.");
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

            Log.Information("Returning packet data.");
            return pkt;
        }

        public void Dispose() => _stream.Dispose();
    }
}
