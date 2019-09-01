using System;

namespace Core
{
    [Flags]
    public enum AuraInterruptFlags
    {
        HitBySpell = 1 << 0,
        AnyDamageTaken = 1 << 1,
        DirectDamageTaken = 1 << 2,
        CombinedDamageTaken = 1 << 3,
        Cast = 1 << 4,
        Move = 1 << 5,
    }
}
