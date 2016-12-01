using System;

namespace BasicRpgEngine
{
    public static class Mechanics
    {
        static Random random = new Random();

        public static int Roll(int min, int max)
        {
            return random.Next(min, max);
        }
    }
}