using System;

namespace Core
{
    [Flags]
    public enum SpellPreventionType
    {
        Silence = 1 << 0,
        Pacify = 1 << 1,
    }
}
