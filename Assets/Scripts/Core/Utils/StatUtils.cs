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

        public static float ModifyMultiplierPercent(float currentValue, float percent, bool apply)
        {
            if (Mathf.Approximately(percent, -100.0f))
                percent = -99.99f;

            return currentValue * (apply ? (100.0f + percent) / 100.0f : 100.0f / (100.0f + percent));
        }
    }
}