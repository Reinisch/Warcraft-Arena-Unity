using System;

namespace Core
{
    /// <summary>
    /// Compressed to 7 bits in <seealso cref="SpellDamageDoneEvent"/> and <seealso cref="UnitSpellDamageEvent"/>
    /// </summary>
    [Flags]
    public enum HitType
    {
        Normal = 1 << 0,
        Miss = 1 << 1,
        FullAbsorb = 1 << 2,
        PartialAbsorb = 1 << 3,
        CriticalHit = 1 << 4,
        Immune = 1 << 5,
    }
}
