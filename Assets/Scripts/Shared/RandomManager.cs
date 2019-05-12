using System;
using System.Collections.Generic;

namespace Core
{
    public static class RandomHelper
    {
        private static Random random = new Random();

        public static bool CheckSuccess(float chance)
        {
            if (chance >= 1)
                return true;

            return random.NextDouble() < chance;
        }

        public static int Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public static float Next(float minValue, float maxValue)
        {
            return minValue + (float)NextDouble() * (maxValue - minValue);
        }

        public static double NextDouble()
        {
            return random.NextDouble();
        }

        public static void SetRandomSeed(int seed)
        {
            random = new Random(seed);
        }

        public static T GetRandomElement<T>(IReadOnlyList<T> elements)
        {
            return elements.Count == 0 ? default : elements[Next(elements.Count)];
        }
    }
}