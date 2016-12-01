using System;

public static class RandomSolver
{
    static Random Random = new Random();

    public static bool CheckSuccess(float chance)
    {
        if (chance >= 1)
            return true;

        return Random.NextDouble() < chance;
    }

    public static int Next(int maxValue)
    {
        return Random.Next(maxValue);
    }

    public static int Next(int minValue, int maxValue)
    {
        return Random.Next(minValue, maxValue);
    }

    public static double NextDouble()
    {
        return Random.NextDouble();
    }

    public static void SetRandomSeed(int seed)
    {
        Random = new Random(seed);
    }
}