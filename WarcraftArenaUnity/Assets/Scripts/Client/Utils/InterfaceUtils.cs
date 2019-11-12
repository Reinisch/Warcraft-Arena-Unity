using System;
using Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Client
{
    public static class InterfaceUtils
    {
        private static readonly char[] TempCharArray = new char[32];
        private static readonly char[] IntChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly char[] MinValue = { '-', '2', '1', '4', '7', '4', '8', '3', '6', '4', '8' };

        public static bool IsPointerOverUI => EventSystem.current.IsPointerOverGameObject();

        public static char[] SetSpellTimerNonAlloc(this char[] charArray, int milliseconds, out int length)
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

        public static char[] SetIntNonAlloc(this char[] charArray, int value, out int length)
        {
            if (charArray.Length < 11)
                Array.Resize(ref charArray, 11);

            if (value == 0)
            {
                charArray[0] = IntChars[0];
                length = 1;
                return charArray;
            }

            if (value == int.MinValue)
            {
                MinValue.CopyTo(charArray, 0);
                length = MinValue.Length;
                return charArray;
            }

            length = 0;
            bool isNegative = value < 0;
            int index = isNegative ? 1 : 0;

            if (isNegative)
            {
                value = -value;
                TempCharArray[0] = '-';
            }

            while (value > 0)
            {
                TempCharArray[index++] = IntChars[value % 10];
                value /= 10;
            }

            TempCharArray.CopyToReverse(charArray, index - 1);
            length = index;
            return charArray;
        }

        public static void SetParentAndReset(this RectTransform thisTransform, RectTransform parentTransform)
        {
            thisTransform.SetParent(parentTransform, false);

            thisTransform.localPosition = Vector3.zero;
            thisTransform.anchoredPosition = Vector3.zero;
            thisTransform.localScale = Vector3.one;
        }
    }
}
