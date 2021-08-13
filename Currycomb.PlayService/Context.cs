using Currycomb.Common.Network;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using System;
using System.Threading.Channels;

using System.Threading.Tasks;
namespace Currycomb.PlayService
{
    public class Context : IGamePacketRouterContext, IMetaPacketRouterContext
    {
        public readonly Guid ClientId;
        public readonly ChannelWriter<IGameEvent> Event;

        private WrappedPacketStream _wps;

        public State State => State.Play;

        public Context(ChannelWriter<IGameEvent> evt, Guid clientId, WrappedPacketStream wps)
            => (Event, ClientId, _wps) = (evt, clientId, wps);

        public async Task SendPacket<T>(T packet) where T : IGamePacket
            => await _wps.SendAsync(new WrappedPacket(ClientId, packet.ToBytes()), false);
    }
}
