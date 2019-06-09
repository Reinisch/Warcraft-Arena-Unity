using System;

namespace Core
{
    [Flags]
    public enum AuraInterruptFlags
    {
        HitBySpell = 1 << 0,
        TakeDamage = 1 << 1,
        Cast = 1 << 2,
        Move = 1 << 3,
        Turning = 1 << 4,
        DirectDamage = 1 << 5,
    }
}
