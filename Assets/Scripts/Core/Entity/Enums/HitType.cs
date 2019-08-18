using System;

namespace Core
{
    [Flags]
    public enum HitType
    {
        Normal = 1 << 0,
        Miss = 1 << 1,
        FullAbsorb = 1 << 2,
        PartialAbsorb = 1 << 3,
        CriticalHit = 1 << 4,
    }
}
