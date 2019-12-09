using System;

namespace Core
{
    [Flags]
    public enum SpellExtraAttributes
    {
        IgnoreCasterAuras = 1 << 0,
        CanTargetInvisible = 1 << 1,
        NotStealable = 1 << 2,
        CanCastWhileCasting = 1 << 3,
        FixedDamage = 1 << 4,
        DamageDoesntBreakAuras = 1 << 5,
        UsableWhileStunned = 1 << 6,
        SingleTargetSpell = 1 << 7,
        UsableWhileFeared = 1 << 8,
        UsableWhileConfused = 1 << 9,
        CastCantCancelShapeShift = 1 << 10,
        CastableOnlyNonShapeShifted = 1 << 11,
        CastableOnlyShapeShifted = 1 << 12,
        DispelCharges = 1 << 13,
        DoesNotTriggerGcd = 1 << 14,
        IgnoreGcd = 1 << 15
    }
}
