using System;
using Common;

namespace Client
{
    public static class InterfaceUtils
    {
        public static char[] SetSpellCooldownNonAlloc(this char[] charArray, int milliseconds, out int length)
        {
            if (charArray.Length < 3)
                Array.Resize(ref charArray, 3);

            if (milliseconds > TimeUtils.DayInMilliseconds)
            {
                int dayCount = milliseconds / TimeUtils.DayInMilliseconds;
                if (dayCount > 9)
                {
                    length = 3;
                    charArray[0] = '9';
                    charArray[1] = 'D';
                    charArray[2] = '+';
                    return charArray;
                }

                length = 2;
                charArray[0] = (char)('0' + dayCount);
                charArray[1] = 'D';
                return charArray;
            }

            if (milliseconds > TimeUtils.HourInMilliseconds)
            {
                int hourCount = milliseconds / TimeUtils.HourInMilliseconds;
                if (hourCount > 9)
                {
                    length = 3;
                    charArray[0] = (char)('0' + hourCount / 10);
                    charArray[1] = (char)('0' + hourCount % 10);
                    charArray[2] = 'H';
                    return charArray;
                }

                length = 2;
                charArray[0] = (char)('0' + hourCount);
                charArray[1] = 'H';
                return charArray;
            }

            if (milliseconds > TimeUtils.MinuteInMilliseconds)
            {
                int minuteCount = milliseconds / TimeUtils.MinuteInMilliseconds;
                if (minuteCount > 9)
                {
                    length = 3;
                    charArray[0] = (char)('0' + minuteCount / 10);
                    charArray[1] = (char)('0' + minuteCount % 10);
                    charArray[2] = 'M';
                    return charArray;
                }

                length = 2;
                charArray[0] = (char)('0' + minuteCount);
                charArray[1] = 'M';
                return charArray;
            }

            if (milliseconds >= TimeUtils.SecondInMilliseconds)
            {
                int secondCount = milliseconds / TimeUtils.SecondInMilliseconds;
                if (secondCount > 9)
                {
                    length = 2;
                    charArray[0] = (char)('0' + secondCount / 10);
                    charArray[1] = (char)('0' + secondCount % 10);
                    return charArray;
                }

                length = 1;
                charArray[0] = (char)('0' + secondCount);
                return charArray;
            }

            length = 3;
            charArray[0] = '0';
            charArray[1] = '.';
            charArray[2] = (char)('0' + milliseconds / 100);
            return charArray;
        }
    }
}
