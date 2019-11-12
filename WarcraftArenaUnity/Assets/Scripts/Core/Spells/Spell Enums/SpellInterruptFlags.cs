using System;

namespace Core
{
    [Flags]
    public enum SpellInterruptFlags
    {
        Movement = 1 << 0,
        PushBack = 1 << 1,
        Interrupt = 1 << 2,
        AbortOnDmg = 1 << 3,
    }
}
