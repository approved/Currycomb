namespace Currycomb.PlayService.Game
{
    public readonly struct GameState
    {
        public readonly float DeltaTime;

        public GameState(float deltaTime)
        {
            DeltaTime = deltaTime;
        }
    }
}
