using System.Runtime.CompilerServices;

namespace Currycomb.Common.Game
{
    public enum GameMode
    {
        None = -1,
        Survival = 0,
        Creative = 1,
        Adventure = 2,
        Spectator = 3
    }

    public static class GameModeExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte AsByte(this GameMode gamemode) => (byte)gamemode;
    }
}
