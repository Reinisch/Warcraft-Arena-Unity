using System;
using System.Collections.Generic;

namespace Common
{
    public static class RandomUtils
    {
        private static Random Random = new Random();

        public static bool CheckSuccess(float chance)
        {
            if (chance >= 1)
                return true;

            return Random.NextDouble() < chance;
        }

        public static bool CheckSuccessPercent(float percent)
        {
            return CheckSuccess(percent / 100.0f);
        }

        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        public static float Next(float minValue, float maxValue)
        {
            return minValue + (float)NextDouble() * (maxValue - minValue);
        }

        public static double NextDouble()
        {
            return Random.NextDouble();
        }

        public static void SetRandomSeed(int seed)
        {
            Random = new Random(seed);
        }

        public static T GetRandomElement<T>(IReadOnlyList<T> elements)
        {
            return elements.Count == 0 ? default : elements[Next(elements.Count)];
        }
    }
}