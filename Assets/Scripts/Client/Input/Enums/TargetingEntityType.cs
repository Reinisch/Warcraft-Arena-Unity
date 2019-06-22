using System;

namespace Client
{
    [Flags]
    public enum TargetingEntityType
    {
        Players = 1 << 0,
        Creatures = 1 << 1
    }
}
