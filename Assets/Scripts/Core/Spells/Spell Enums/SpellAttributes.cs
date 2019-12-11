using System;

namespace Core
{
    [Flags]
    public enum SpellAttributes
    {
        Passive = 1 << 0,
        CastableWhileDead = 1 << 1,
        CastableWhileMoving = 1 << 2,
        UnaffectedByInvulnerability = 1 << 3,
        CantCancel = 1 << 4,
        CantBeUsedInCombat = 1 << 5,
        CantBeReflected = 1 << 6,
        DispelAurasOnImmunity = 1 << 7,
        UnaffectedBySchoolImmune = 1 << 8,
        CantTargetSelf = 1 << 9,
        CanTargetDead = 1 << 10,
        CantCrit = 1 << 11,
        AlwaysCrit = 1 << 12,
        IgnoreSpellModifiers = 1 << 13,
        CantTriggerProc = 1 << 14,
        DisableProc = 1 << 15,
        BlockableSpell = 1 << 16,
        StackForDiffCasters = 1 << 17,
        OnlyTargetPlayers = 1 << 18,
        IgnoreHitResult = 1 << 19,
        DeathPersistent = 1 << 20,
        RequiresComboPoints = 1 << 21
    }
}
