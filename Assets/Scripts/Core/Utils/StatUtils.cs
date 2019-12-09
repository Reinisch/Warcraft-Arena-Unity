using System;
using UnityEngine;

namespace Core
{
    public static class StatUtils
    {
        public const float DefaultEntitySize = 0.2f;
        public const float DefaultCombatReach = 1.5f;
        public const float MinMeleeReach = 3.0f;
        public const float NominalMeleeRange = 5.0f;

        public static readonly UnitMoveType[] UnitMoveTypes = (UnitMoveType[]) Enum.GetValues(typeof(UnitMoveType));
        public static readonly StatType[] UnitStatTypes = (StatType[])Enum.GetValues(typeof(StatType));
        public static readonly UnitModifierType[] UnitModTypes = (UnitModifierType[])Enum.GetValues(typeof(UnitModifierType));

        public static float ModifyMultiplierPercent(float currentValue, float percent, bool apply)
        {
            if (Mathf.Approximately(percent, -100.0f))
                percent = -99.99f;

            return currentValue * (apply ? (100.0f + percent) / 100.0f : 100.0f / (100.0f + percent));
        }

        public static SpellPowerType AsPower(this UnitModifierType modType) => (SpellPowerType)(modType - UnitModifierType.StartPowers);

        public static UnitModifierType AsModifier(this StatType statType) => (UnitModifierType) (statType + (int)UnitModifierType.StartStats);

        public static UnitModifierType AsModifier(this SpellPowerType spellPowerType) => (UnitModifierType) (spellPowerType + (int)UnitModifierType.StartPowers);
    }
}