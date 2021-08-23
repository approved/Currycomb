using Currycomb.Common.Network.Game.Packets.Types;
using Currycomb.PlayService.Game.Component;
using DefaultEcs;
using DefaultEcs.System;

namespace Currycomb.PlayService.Game.System
{
    public readonly struct GameEntitySpawning { }

    public sealed partial class EntitySpawnSystem : AEntitySetSystem<GameState>
    {
        [WorldComponent]
        private PacketSender _pkt;

        [WorldComponent]
        private SafeRandom _rand;

        [Update, UseBuffer]
        private void Update(in Entity entity, ref GameEntitySpawning spawn)
        {
            entity.Remove<GameEntitySpawning>();

            entity.Set<GameEntityId>(new(_rand.Next()));
            entity.Set<GameEntityGuid>(GameEntityGuid.New());

            // TODO Read from GameEntitySpawning
            entity.Set<EntityType>(EntityType.Player);
            entity.Set<Position>(new());
            entity.Set<Rotation>(new());
            entity.Set<GameEntityObjectData>(new());
            entity.Set<Velocity>(new());
        }
    }
}
