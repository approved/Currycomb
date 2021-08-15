using System;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;
using Currycomb.Gateway.Clients;

namespace Currycomb.Gateway.Meta
{
    public class MetaContext : IMetaPacketRouterContext, IGamePacketRouterContext
    {
        public readonly Guid ClientId;
        private ClientConnection _client;

        public MetaContext(ClientConnection client) => _client = client;

        public State State => _client.State;

        public void SetAesKey(byte[] aesKey) => _client.SetAesKey(aesKey);
        public void SetState(State state) => _client.SetState(state);
    }
}
