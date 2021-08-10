using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Broadcast;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta.Packets;

namespace Currycomb.AuthService
{
    public class Context : IMetaPacketRouterContext, IGamePacketRouterContext
    {
        public readonly Guid ClientId;
        public readonly RSA Rsa;

        public readonly byte[] VerifyToken;

        public State State => _state.TryGetValue(ClientId, out var x) ? x : State.Handshake;

        private IDictionary<Guid, State> _state;
        private WrappedPacketStream _wps;
        private ClientWebSocket _events;

        public Context(Guid clientId, RSA rsa, IDictionary<Guid, State> state, WrappedPacketStream wps, ClientWebSocket events)
        {
            ClientId = clientId;
            Rsa = rsa;
            _state = state;
            _wps = wps;
            _events = events;

            VerifyToken = clientId.ToByteArray()[0..4];
        }

        public async Task SetState(State state)
        {
            await Broadcast(EventType.ChangedState, new PayloadStateChange(ClientId, _state[ClientId] = state));
            await SendMetaPacket(new PacketSetState(state));
        }

        public async Task SendMetaPacket<T>(T packet) where T : IMetaPacket
            => await _wps.SendWaitAsync(new WrappedPacket(ClientId, packet.ToBytes()), true);

        public async Task SendPacket<T>(T packet) where T : IGamePacket
            => await _wps.SendAsync(new WrappedPacket(ClientId, packet.ToBytes()), false);

        public async Task Broadcast(ComEvent ev)
            => await _events.SendAsync(Encoding.UTF8.GetBytes(ev.Serialize()), WebSocketMessageType.Text, true, default);

        public Task Broadcast<T>(EventType evt, T data)
            => Broadcast(ComEvent.Create(evt, data));
    }
}
