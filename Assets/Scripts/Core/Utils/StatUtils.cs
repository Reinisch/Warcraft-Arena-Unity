using System;
using UnityEngine;

namespace Core
{
    public static class StatUtils
    {
        public const float DefaultCombatReach = 1.5f;
        public const float MinMeleeReach = 3.0f;
        public const float NominalMeleeRange = 5.0f;

        public static readonly UnitMoveType[] UnitMoveTypes = (UnitMoveType[]) Enum.GetValues(typeof(UnitMoveType));
        public static readonly StatType[] UnitStatTypes = (StatType[])Enum.GetValues(typeof(StatType));

        public static bool HasTargetFlag(this UnitFlags baseFlags, UnitFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

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

        public static float ModifyMultiplierPercent(float currentValue, float percent, bool apply)
        {
            if (Mathf.Approximately(percent, -100.0f))
                percent = -99.99f;

            return currentValue * (apply ? (100.0f + percent) / 100.0f : 100.0f / (100.0f + percent));
        }

        #region Binary helpers

        public static bool HasBit(this int mask, int index) => (mask & (1 << index)) > 0;

        public static int SetBit(this int mask, int index) => mask | (1 << index);

        public static int SetBit(this int mask, int index, bool set) => set ? mask | (1 << index) : mask & ~(1 << index);

        #endregion
    }
}