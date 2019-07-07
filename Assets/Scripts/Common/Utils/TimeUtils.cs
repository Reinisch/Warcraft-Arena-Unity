using System;

namespace Common
{
    public static class TimeUtils
    {
        public const int Second = 1;
        public const int Minute = Second * 60;
        public const int Hour = Minute * 60;
        public const int Day = Hour * 24;
        public const int Week = Day * 7;
        public const int Month = Day * 30;
        public const int Year = Month * 12;
        public const int InMilliseconds = 1000;

        public const int SecondInMilliseconds = Second * InMilliseconds;
        public const int MinuteInMilliseconds = Minute * InMilliseconds;
        public const int HourInMilliseconds = Hour * InMilliseconds;
        public const int DayInMilliseconds = Day * InMilliseconds;

        public static long NowInMilliseconds => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }
}