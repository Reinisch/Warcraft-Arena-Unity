using System;

namespace Core
{
    public static class TimeHelper
    {
        public const int Minute = 60;
        public const int Hour = Minute * 60;
        public const int Day = Hour * 24;
        public const int Week = Day * 7;
        public const int Month = Day * 30;
        public const int Year = Month * 12;
        public const int InMilliseconds = 1000;

        public static long NowInMilliseconds => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
}