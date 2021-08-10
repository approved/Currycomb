using Currycomb.Common.Network;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Game;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Channels;

using System.Threading.Tasks;
namespace Currycomb.PlayService
{
    public class Context : IGamePacketRouterContext
    {
        public readonly Guid ClientId;
        public readonly ChannelWriter<IGameEvent> Event;

        private WrappedPacketStream _wps;
        private ClientWebSocket _events;

        public State State => State.Play;

        public Context(ChannelWriter<IGameEvent> evt, Guid clientId, WrappedPacketStream wps, ClientWebSocket events)
            => (Event, ClientId, _wps, _events) = (evt, clientId, wps, events);

        public Task SetState(State state)
            => Broadcast(EventType.ChangedState, new PayloadStateChange(ClientId, state));

        public async Task SendPacket<T>(T packet) where T : IGamePacket
            => await _wps.SendAsync(new WrappedPacket(ClientId, packet.ToBytes()), false);

        public async Task Broadcast(ComEvent ev)
            => await _events.SendAsync(Encoding.UTF8.GetBytes(ev.Serialize()), WebSocketMessageType.Text, true, default);

        public Task Broadcast<T>(EventType evt, T data)
            => Broadcast(ComEvent.Create(evt, data));
    }
}
