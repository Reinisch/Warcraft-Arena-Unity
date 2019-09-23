using System;
using UnityEngine;

namespace Common
{
    public static class MathUtils
    {
        public static bool CompareWith(this float self, float other, ComparisonOperator comparison)
        {
            switch (comparison)
            {
                case ComparisonOperator.Equal:
                    return Mathf.Approximately(self, other);
                case ComparisonOperator.NotEqual:
                    return !Mathf.Approximately(self, other);
                case ComparisonOperator.Less:
                    return self < other;
                case ComparisonOperator.LessOrEqual:
                    return self <= other;
                case ComparisonOperator.Greater:
                    return self > other;
                case ComparisonOperator.GreaterOrEqual:
                    return self >= other;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, "Unknown comparison type!");
            }
        }

        public static bool CompareWith(this int self, int other, ComparisonOperator comparison)
        {
            switch (comparison)
            {
                case ComparisonOperator.Equal:
                    return self == other;
                case ComparisonOperator.NotEqual:
                    return self != other;
                case ComparisonOperator.Less:
                    return self < other;
                case ComparisonOperator.LessOrEqual:
                    return self <= other;
                case ComparisonOperator.Greater:
                    return self > other;
                case ComparisonOperator.GreaterOrEqual:
                    return self >= other;
                default:
                    throw new ArgumentOutOfRangeException(nameof(comparison), comparison, "Unknown comparison type!");
            }
        }

        public static bool HasBit(this int mask, int index) => (mask & (1 << index)) > 0;

        public static int SetBit(this int mask, int index) => mask | (1 << index);

        public static int SetBit(this int mask, int index, bool set) => set ? mask | (1 << index) : mask & ~(1 << index);

        #region Percentage calculations

        public static int CalculatePercentage(this int baseValue, float percent)
        {
            return (int)(baseValue * percent / 100.0f);
        }

        public static uint CalculatePercentage(this uint baseValue, float percent)
        {
            return (uint)(baseValue * percent / 100.0f);
        }

        public static long CalculatePercentage(this long baseValue, float percent)
        {
            return (long)(baseValue * percent / 100.0f);
        }

        public static float CalculatePercentage(this float baseValue, float percent)
        {
            return baseValue * (percent) / 100.0f;
        }

        public static int AddPercentage(this int baseValue, float percent)
        {
            return baseValue + CalculatePercentage(baseValue, percent);
        }

        public static uint AddPercentage(this uint baseValue, float percent)
        {
            return baseValue + CalculatePercentage(baseValue, percent);
        }

        public static long AddPercentage(this long baseValue, float percent)
        {
            return baseValue + CalculatePercentage(baseValue, percent);
        }

        public static float AddPercentage(this float baseValue, float percent)
        {
            return baseValue + CalculatePercentage(baseValue, percent);
        }

        public static int ApplyPercentage(this int baseValue, float percent)
        {
            return CalculatePercentage(baseValue, percent);
        }

        public static uint ApplyPercentage(this uint baseValue, float percent)
        {
            return CalculatePercentage(baseValue, percent);
        }

        public static long ApplyPercentage(this long baseValue, float percent)
        {
            return CalculatePercentage(baseValue, percent);
        }

        public static float ApplyPercentage(this float baseValue, float percent)
        {
            return CalculatePercentage(baseValue, percent);
        }

        #endregion
    }
}
