using Currycomb.PlayService.Game.Component;
using DefaultEcs;
using DefaultEcs.System;

namespace Currycomb.PlayService.Game.System
{
    public sealed partial class MovementSystem : AEntitySetSystem<GameState>
    {
        [Update]
        private static void Update(in Entity entity, ref Position position, in MoveTo targetPos)
        {
            position.Value += targetPos.Value;
        }
    }
}
