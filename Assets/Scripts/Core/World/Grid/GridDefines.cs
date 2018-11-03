using System;

namespace Core
{
    public enum GridStateType
    {
        Invalid = 0,
        Active = 1,
        Idle = 2,
        Removal = 3,

        Max = 4
    }

    [Flags]
    public enum GridMapTypeMask
    {
        Corpse = 0x01,
        Creature = 0x02,
        Dynamicobject = 0x04,
        Gameobject = 0x08,
        Player = 0x10,
        Areatrigger = 0x20,

        All = 0x3F
    }
}