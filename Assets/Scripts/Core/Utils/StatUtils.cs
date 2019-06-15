using System;

namespace Core
{
    public static class StatUtils
    {
        public const float ContactDistance = 0.5f;
        public const float InteractionDistance = 5.0f;
        public const float AttackDistance = 5.0f;
        public const float InspectDistance = 28.0f;
        public const float TradeDistance = 11.11f;
        public const float SightRangeUnit = 50.0f;
        public const float DefaultCombatReach = 1.5f;
        public const float MinMeleeReach = 2.0f;
        public const float NominalMeleeRange = 5.0f;
        public const float MeleeRange = NominalMeleeRange - MinMeleeReach * 2;
        public const float DefaultPhase = 169;
        public const int CorpseReclaimRadius = 50;
        public const int AIDefaultCooldown = 5000;
        public static readonly UnitMoveType[] UnitMoveTypes = (UnitMoveType[]) Enum.GetValues(typeof(UnitMoveType));

        public static bool HasTargetFlag(this UnitFlags baseFlags, UnitFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        #region Percentage calculations

        public static int CalculatePercentage(this int baseValue, float pct)
        {
            return (int)(baseValue * pct / 100.0f);
        }

        public static uint CalculatePercentage(this uint baseValue, float pct)
        {
            return (uint)(baseValue * pct / 100.0f);
        }

        public static long CalculatePercentage(this long baseValue, float pct)
        {
            return (long)(baseValue * pct / 100.0f);
        }

        public static float CalculatePercentage(this float baseValue, float pct)
        {
            return baseValue * (pct) / 100.0f;
        }

        public static int AddPercentage(this int baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static uint AddPercentage(this uint baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static long AddPercentage(this long baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static float AddPercentage(this float baseValue, float pct)
        {
            return baseValue + CalculatePercentage(baseValue, pct);
        }

        public static int ApplyPercentage(this int baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        public static uint ApplyPercentage(this uint baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        public static long ApplyPercentage(this long baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        public static float ApplyPercentage(this float baseValue, float pct)
        {
            return CalculatePercentage(baseValue, pct);
        }

        #endregion
    }
}