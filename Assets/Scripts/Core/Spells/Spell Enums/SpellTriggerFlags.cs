using System;

namespace Core
{
    [Flags]
    public enum SpellTriggerFlags
    {
        Kill = 1 << 0,
        DoneSpellDamage = 1 << 1,
        TakenSpellDamage = 1 << 2,
        DonePeriodicDamage = 1 << 3,
        TakenPeriodicDamage = 1 << 4,
        DoneSpellCast = 1 << 5,
        DoneSpellHit = 1 << 6,
        TakenSpellHit = 1 << 7,
    }
}
