using System.Runtime.CompilerServices;

namespace Currycomb.Common.Game
{
    public enum Difficulty : sbyte
    {
        Invalid = -1,
        Peaceful = 0,
        Easy = 1,
        Normal = 2,
        Hard = 3
    }

    public static class DifficultyExt
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte AsByte(this Difficulty difficulty) => (byte)difficulty;
    }
}
