using Currycomb.Common.Network.Game.Packets;
using Currycomb.Common.Network.Game.Packets.Types;
using Currycomb.PlayService.Game.Component;
using DefaultEcs.System;

namespace Currycomb.PlayService.Game.System
{
    public sealed partial class NetworkOutputEntitySpawnedSystem : AEntitySetSystem<GameState>
    {
        [WorldComponent]
        private PacketSender _pkt;

        [Update]
        public void Update([Added] in GameEntityId id, in GameEntityGuid guid, in EntityType type, in Position position, in Rotation rotation, in GameEntityObjectData data, in Velocity velocity)
        {
            _pkt.Broadcast<PacketSpawnEntity>(new(
                id.Value,
                guid.Value,
                type,
                position.Value.X,
                position.Value.Y,
                position.Value.Z,
                rotation.Pitch,
                rotation.Yaw,
                data.Value,
                velocity.X,
                velocity.Y,
                velocity.Z));
        }
    }
}