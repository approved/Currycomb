using System;
using System.Collections.Concurrent;
using Currycomb.Common.Network.Game.Packets;
using Currycomb.Common.Network.Game.Packets.Types;
using Currycomb.PlayService.Game.Component;
using DefaultEcs;
using DefaultEcs.System;

namespace Currycomb.PlayService.Game.System
{

    public sealed class EntityMovedSystem : AEntitySetSystem<GameState>
    {
        private readonly IDisposable _subscription;
        private readonly World _world;

        private readonly PacketSender _pkt;

        public EntityMovedSystem(World world) : base(world.GetEntities().With<GameEntityId>().WhenChanged<Position>().AsSet())
        {
            _world = world;

            _pkt = world.Get<PacketSender>();

            _subscription = world.SubscribeComponentChanged<Position>(OnEntityMoved);
        }

        protected override void Update(GameState state, in Entity entity)
        {
            var id = entity.Get<GameEntityId>();
            if (_moved.TryRemove(id, out var moved))
            {
                var from = moved.From.Value;
                var to = moved.To.Value;

                // TODO: only teleport when moving "more than 8 blocks"
                _pkt.Broadcast(new PacketTeleportEntity(
                    id.Value,
                    to.X,
                    to.Y,
                    to.Z,
                    new NetAngle(0),
                    new NetAngle(0),
                    false));
            }
        }

        protected override void PostUpdate(GameState state)
        {
            Set.Complete();
        }

        public override void Dispose()
        {
            _subscription.Dispose();
            base.Dispose();
        }

        private readonly ConcurrentDictionary<GameEntityId, (Position From, Position To)> _moved = new();
        private void OnEntityMoved(in Entity entity, in Position oldValue, in Position newValue)
        {
            if (entity.Has<GameEntityId>())
                _moved[entity.Get<GameEntityId>()] = (oldValue, newValue);
        }
    }
}
