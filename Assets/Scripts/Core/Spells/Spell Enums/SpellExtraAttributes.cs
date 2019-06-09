using System;

namespace Core
{
    [Flags]
    public enum SpellExtraAttributes
    {
        IgnoreResistances = 1 << 0,
        ProcOnlyOnCaster = 1 << 1,
        NotStealable = 1 << 2,
        CanCastWhileCasting = 1 << 3,
        FixedDamage = 1 << 4,
        DamageDoesntBreakAuras = 1 << 5,
        UsableWhileStunned = 1 << 6,
        SingleTargetSpell = 1 << 7,
        UsableWhileFeared = 1 << 8,
        UsableWhileConfused = 1 << 9,
        CanTargetInvisible = 1 << 10,
        OnlyVisibleToCaster = 1 << 11,
        CanTargetUntargetable = 1 << 12,
        DispelCharges = 1 << 13,
        SpecialDelayCalculation = 1 << 14,
        DisabledWhileActive = 1 << 15
    }
}
