using System;

namespace Core
{
    [Flags]
    public enum PlayerExtraFlags
    {
        // gm abilities
        GmOn = 0x0001,
        AcceptWhispers = 0x0004,
        Taxicheat = 0x0008,
        Invisible = 0x0010,
        GmChat = 0x0020,               // Show GM badge in chat messages

        // other states
        PvpDeath = 0x0100                // store PvP death status until corpse creating.
    }

    public enum EnviromentalDamage : byte
    {
        Exhausted = 0,
        Drowning = 1,
        Fall = 2,
        Lava = 3,
        Slime = 4,
        Fire = 5,
        FallToVoid = 6
    }
}