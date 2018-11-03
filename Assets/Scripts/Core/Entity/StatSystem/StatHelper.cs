using System.Collections.Generic;

namespace Core
{
    public static class StatHelper
    {
        public const int MaxStats = 4;


        public static void InitializeSpeedRates(Dictionary<UnitMoveType, float> speedRates)
        {
            speedRates.Clear();

            foreach (var speedRate in baseMoveSpeed)
                speedRates.Add(speedRate.Key, 1.0f);
        }

        public static float BaseMovementSpeed(UnitMoveType moveType)
        {
            return baseMoveSpeed[moveType];
        }


        public static long MakePair(uint l, uint h)
        {
            return l | ((long)h << 32);
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

        #region Unit speed info

        private static Dictionary<UnitMoveType, float> baseMoveSpeed = new Dictionary<UnitMoveType, float>()
        {
            { UnitMoveType.Run, 7.0f },
            { UnitMoveType.RunBack, 4.5f },
            { UnitMoveType.TurnRate, 3.14f },
            { UnitMoveType.PitchRate, 3.14f },
        };

        #endregion
    }
}