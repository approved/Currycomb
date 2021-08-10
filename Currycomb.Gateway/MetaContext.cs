using System;
using Currycomb.Common.Network.Meta;
using Currycomb.Common.Network.Game;
using Currycomb.Gateway.ClientData;

namespace Currycomb.Gateway
{
    public class MetaContext : IMetaPacketRouterContext, IGamePacketRouterContext
    {
        public readonly Guid ClientId;
        public State State => _client.State;
        private ClientConnection _client;

        public MetaContext(ClientConnection client) => _client = client;

        public void SetState(State state) => _client.SetState(state);
        public void SetAesKey(byte[] aesKey) => _client.SetAesKey(aesKey);
    }
}
