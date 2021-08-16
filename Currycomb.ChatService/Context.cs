using System;
using System.Threading.Tasks;
using Currycomb.Common.Network;
using Currycomb.Common.Network.Game;
using Currycomb.Common.Network.Meta;

namespace Currycomb.ChatService
{
    public class Context : IGamePacketRouterContext, IMetaPacketRouterContext
    {
        public readonly Guid ClientId;

        private WrappedPacketStream _wps;

        public Context(Guid clientId, WrappedPacketStream wps)
            => (ClientId, _wps) = (clientId, wps);

        public State State => State.Play;

        public async Task SendPacket<T>(T packet) where T : IGamePacket
            => await _wps.SendAsync(false, new WrappedPacket(ClientId, packet.ToBytes()));

        public async Task BroadcastPacket<T>(T packet) where T : IGamePacket
            => await _wps.SendAsync(false, new WrappedPacket(Common.Constants.ClientId.BroadcastGuid, packet.ToBytes()));
    }
}
