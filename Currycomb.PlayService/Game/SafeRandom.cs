using System;

namespace Currycomb.PlayService.Game
{
    public class SafeRandom
    {
        private readonly Random random = new Random();
        private readonly object syncRoot = new object();

        public int Next()
        {
            lock (syncRoot)
                return random.Next();
        }

        public int Next(int maxValue)
        {
            lock (syncRoot)
                return random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            lock (syncRoot)
                return random.Next(minValue, maxValue);
        }
    }
}
