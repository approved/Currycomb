namespace Currycomb.PlayService.Game.Component
{
    public readonly struct GameEntityId
    {
        public readonly int Value;
        public GameEntityId(in int value) { Value = value; }
    }
}
