using System;

namespace Currycomb.PlayService.Game.Component
{
    public readonly struct GameEntityGuid
    {
        public readonly Guid Value;

        public GameEntityGuid(Guid value)
        {
            Value = value;
        }

        public static GameEntityGuid New()
            => new GameEntityGuid(Guid.NewGuid());
    }
}