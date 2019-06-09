using System;

namespace Core
{
    [Flags]
    public enum SpellRangeFlags
    {
        Default = 0,
        Melee = 1,
        Ranged = 2,
    }
}
