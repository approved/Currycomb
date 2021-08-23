using System;
using System.Collections.Concurrent;
using Currycomb.Common.Network.Game.Packets;
using Currycomb.PlayService.Game.Component;
using Currycomb.PlayService.Game.Message;
using DefaultEcs;
using DefaultEcs.System;
using DefaultEcs.Threading;

namespace Currycomb.PlayService.Game.System
{
    public sealed class NetworkInputMoveSystem : AEntitySetSystem<GameState>
    {
        private readonly IDisposable _subscription;

        public NetworkInputMoveSystem(World world, IParallelRunner runner)
            : base(world.GetEntities().With<ClientId>().With<MoveTo>().AsSet(), runner)
        {
            _subscription = world.Subscribe<Pkt<PacketClientMovePlayerPos>>(On);
        }

        private ConcurrentDictionary<ClientId, PacketClientMovePlayerPos> _queue = new();
        private void On(in Pkt<PacketClientMovePlayerPos> pkt) => _queue[pkt.Source] = pkt.Data;

        protected override void Update(GameState state, in Entity entity)
        {
            var clientId = entity.Get<ClientId>();
            if (_queue.TryRemove(clientId, out var move))
            {
                ref var position = ref entity.Get<MoveTo>();
                position.Value.X = (float)move.XPos;
                position.Value.Y = (float)move.YPos;
                position.Value.Z = (float)move.ZPos;
                entity.NotifyChanged<MoveTo>();
            }
        }

        public override void Dispose()
        {
            _subscription.Dispose();
            base.Dispose();
        }
    }
}
