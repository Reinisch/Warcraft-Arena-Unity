using System;
using System.Collections.Generic;
using System.Text;

namespace ElleRealTime.Shared
{
    public class Constants
    {
        public const string DATE_FORMAT = "dd/MM/yyyy";
        public const string DATE_FORMAT_NOCHAR = "ddMMyyyy";
        public const string DATETIME_FORMAT = "dd/MM/yyyy HH:mm:ss";
        public const string HOUR_FORMAT = "HH:mm:ss";

        public static long UnixTimeNow()
        {
            /*var timeSpan = (DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return (long)timeSpan;*/
            long CurrentTimestamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0)).Ticks;
            return CurrentTimestamp;
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }


    }

    public enum ThreeStatesFilter
    {
        All = 0,
        OnlyTrue = 1,
        OnlyFalse = 2
    }
}
