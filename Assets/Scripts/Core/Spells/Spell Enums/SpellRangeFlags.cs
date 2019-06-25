using System;

namespace Core
{
    [Flags]
    public enum SpellRangeFlags
    {
        Default = 1 << 0,
        Melee = 1 << 1,
        Ranged = 1 << 2,
    }
}
