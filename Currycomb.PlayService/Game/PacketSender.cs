using System;
using System.Threading.Channels;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Microsoft.IO;
using Serilog;

namespace Currycomb.PlayService.Game
{
    public struct PacketSender
    {
        private RecyclableMemoryStreamManager _msManager;
        private ChannelWriter<WrappedPacket> _packetWriter;

        public PacketSender(ChannelWriter<WrappedPacket> packetWriter)
        {
            _msManager = new RecyclableMemoryStreamManager();
            _packetWriter = packetWriter;
        }

        public void Send(WrappedPacket packet)
        {
            if (!_packetWriter.TryWrite(packet))
                throw new InvalidOperationException("PacketSender.Send: Failed to send packet");
        }

        public void Send<T>(Guid clientId, T packet) where T : IGamePacket
        {
            Log.Information("Sending packet {packet} to client {clientId}", packet, clientId);
            if (!_packetWriter.TryWrite(new WrappedPacket(clientId, packet.ToBytes(_msManager.GetStream()))))
                throw new InvalidOperationException("PacketSender.Send: Failed to send packet");
        }

        public void Broadcast<T>(T packet) where T : IGamePacket
        {
            Log.Information("Broadcasting packet {packet}", packet);
            if (!_packetWriter.TryWrite(new WrappedPacket(Common.Constants.ClientIdBroadcast, packet.ToBytes(_msManager.GetStream()))))
                throw new InvalidOperationException("PacketSender.Broadcast: Failed to send packet");
        }
    }
}
