using System;

namespace Confirmit.CATI.Common.Random
{
    public class Randomizer
    {
        private static readonly System.Random Random = new System.Random(Guid.NewGuid().GetHashCode());

        public static int Next(int minValue, int maxValue)
        {
            lock (Random)
            {
                return Random.Next(minValue, maxValue);
            }
        }

        public static int Next(int maxValue)
        {
            lock (Random)
            {
                return Random.Next(maxValue);
            }
        }

        public static double NextDouble()
        {
            lock (Random)
            {
                return Random.NextDouble();
            }
        }

        public static int Next()
        {
            lock (Random)
            {
                return Random.Next();
            }
        }
    }
}
