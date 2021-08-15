using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Currycomb.Common.Network;
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

        public Context(Guid clientId, RSA rsa, IDictionary<Guid, State> state, WrappedPacketStream wps)
        {
            ClientId = clientId;
            Rsa = rsa;
            _state = state;
            _wps = wps;

            VerifyToken = clientId.ToByteArray()[0..4];
        }

        public Task SetState(State state)
            => SendMetaPacket(new PacketSetState(_state[ClientId] = state));

        public Task SendMetaPacket<T>(T packet) where T : IMetaPacket
            => SendMetaPacket<T>(new MetaPacket<T>(new(MetaPacketIdMap<T>.Id), packet));

        public async Task SendMetaPacket<T>(MetaPacket<T> packet) where T : IMetaPacket
            => await _wps.SendWaitAsync(true, new WrappedPacket(ClientId, packet.ToBytes()));

        public async Task SendPacket<T>(T packet) where T : IGamePacket
            => await _wps.SendAsync(false, new WrappedPacket(ClientId, packet.ToBytes()));
    }
}
