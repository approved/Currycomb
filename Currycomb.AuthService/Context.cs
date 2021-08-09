using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Minecraft;

namespace Currycomb.AuthService
{
    public class Context : IPacketRouterContext
    {
        public readonly Guid ClientId;
        public State State => _state.TryGetValue(ClientId, out var x) ? x : State.Handshake;

        private IDictionary<Guid, State> _state;
        private WrappedPacketStream _wps;
        private ClientWebSocket _events;

        public Context(Guid clientId, IDictionary<Guid, State> state, WrappedPacketStream wps, ClientWebSocket events)
            => (ClientId, _state, _wps, _events) = (clientId, state, wps, events);

        public Task SetState(State state)
            => Broadcast(EventType.ChangedState, new PayloadStateChange(ClientId, _state[ClientId] = state));

        public async Task SendPacket<T>(T packet) where T : IPacket
            => await _wps.SendAsync(new WrappedPacket(ClientId, packet.ToBytes()));

        public async Task Broadcast(ComEvent ev)
            => await _events.SendAsync(Encoding.UTF8.GetBytes(ev.Serialize()), WebSocketMessageType.Text, true, default);

        public Task Broadcast<T>(EventType evt, T data)
            => Broadcast(ComEvent.Create(evt, data));
    }
}
