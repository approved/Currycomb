using Currycomb.Common.Extensions;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Meta.Packets;
using Microsoft.IO;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Reflection.Metadata;
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

        readonly Stream _stream;
        readonly RecyclableMemoryStreamManager _msManager;
        readonly ConcurrentDictionary<Guid, TaskCompletionSource<byte>> _blocking = new();
        readonly Channel<WrappedPacketContainer> _queuedPackets = Channel.CreateUnbounded<WrappedPacketContainer>();
        readonly Channel<(Guid, byte)> _queuedAcks = Channel.CreateUnbounded<(Guid, byte)>();

        public WrappedPacketStream(Stream stream, RecyclableMemoryStreamManager? msManager = null)
            => (_stream, _msManager) = (stream, msManager ?? new());

        private MemoryStream GetMemoryStream()
            => _msManager.GetStream("WrappedPacketStream");

        private BinaryWriter GetBinaryWriter()
            => new(GetMemoryStream());

        private async Task WritePacketAsync(Action<BinaryWriter> action)
        {
            using var writer = GetBinaryWriter();
            action(writer);
            writer.BaseStream.Position = 0;
            await writer.BaseStream.CopyToAsync(_stream);
        }

        public Task<WrappedPacketContainer> RunOnceAsync(bool handleAck, CancellationToken ct = default)
        {
            byte[] ackBuffer = new byte[16];
            byte[] metaBuffer = new byte[1];

            return RunOnceAsync(handleAck, handleAck, ackBuffer, metaBuffer, ct);
        }

        // Note that handling ack but not queued ack at any point where you are not already sure that there are no queued acks will
        // result in acks being sent out of sync. This will most likely never matter, but if it ever DOES end up mattering and happening
        // the problem is most likely that someone is not passing true, false
        private async Task<WrappedPacketContainer> RunOnceAsync(bool handleAck, bool handleQueuedAck, byte[] ackBuffer, byte[] metaBuffer, CancellationToken ct = default)
        {
            log.Information("WPS.RunAsync | Waiting for incoming packet (idle)");

            if (handleQueuedAck)
                while (_queuedAcks.Reader.TryRead(out var ack))
                    HandleAck(ack.Item1, ack.Item2);

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                await _stream.ReadAsync(metaBuffer.AsMemory(0, 1), ct);
                byte meta = metaBuffer[0];

                if ((meta & META_ACK_RES) == 0)
                {
                    Guid? acknowledgement = (meta & META_ACK_REQ) != 0 ? await _stream.ReadGuidAsync() : null;
                    bool isInternalPacket = (meta & META_INT_PKT) != 0;

                    log.Information("WPS.RunAsync | Reading incoming wrapped packet {ack}", acknowledgement);
                    return new WrappedPacketContainer(acknowledgement, isInternalPacket, await WrappedPacket.ReadAsync(_stream));
                }
                else
                {
                    await _stream.ReadAsync(ackBuffer.AsMemory(0, 16), ct);
                    Guid ackGuid = new Guid(ackBuffer);
                    byte ackVal = (byte)_stream.ReadByte();

                    if (handleAck)
                        HandleAck(ackGuid, ackVal);
                    else
                        await _queuedAcks.Writer.WriteAsync((ackGuid, ackVal));
                }
            }
        }

        private void HandleAck(Guid ackGuid, byte ackVal)
        {
            log.Information($"WPS.RunAsync | ACK for packet received: {ackGuid}");
            if (!_blocking.TryRemove(ackGuid, out var tcs) || !tcs.TrySetResult(ackVal))
                throw new InvalidOperationException("Failed to handle incoming ACK packet.");
        }

        public async Task RunAsync(CancellationToken ct = default)
        {
            byte[] ackBuffer = new byte[16];
            byte[] metaBuffer = new byte[1];

            await _queuedPackets.Writer.WriteAsync(await RunOnceAsync(true, true, ackBuffer, metaBuffer, ct));

            while (!ct.IsCancellationRequested)
            {
                // TODO: Handle errors
                await _queuedPackets.Writer.WriteAsync(await RunOnceAsync(true, false, ackBuffer, metaBuffer, ct));
                log.Information($"WPS.RunAsync | Queued incoming wrapped packet");
            }
        }

        public Task Announce(PacketAnnounce announce)
            => this.SendWaitAsync(true, new WrappedPacket(Common.Constants.ClientId.ToGatewayGuid, new MetaPacket<PacketAnnounce>(new(MetaPacketIdMap<PacketAnnounce>.Id), announce).ToBytes()));

        public async Task<byte> SendWaitAsync(bool isMeta, WrappedPacket packet)
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

        public Task SendAsync(bool isMeta, WrappedPacket packet)
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

        public async ValueTask SendAckAsync(WrappedPacketContainer pktc, byte data = 255)
        {
            if (pktc.AckGuid is Guid id)
                await SendAckAsync(id, data);
        }

        public async Task<WrappedPacketContainer> ReadAsync(bool autoAck = false, CancellationToken ct = default)
        {
            WrappedPacketContainer pktc = await _queuedPackets.Reader.ReadAsync(ct);
            log.Information("Received packet {guid}", pktc.AckGuid);

            if (!autoAck || !pktc.AckGuid.HasValue)
                return pktc;

            await SendAckAsync(pktc.AckGuid.Value);
            return pktc with { AckGuid = null };
        }

        public void Dispose()
        {
            _stream.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
