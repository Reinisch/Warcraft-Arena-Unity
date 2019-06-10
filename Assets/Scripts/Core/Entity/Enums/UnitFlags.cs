using System;

namespace Core
{
    [Flags]
    public enum UnitFlags
    {
        ServerControlled = 1 << 0,
        NonAttackable = 1 << 1,
        DisableMove = 1 << 2,
        PvpAttackable = 1 << 3,
        Preparation = 1 << 4,
        Looting = 1 << 5,
        Pvp = 1 << 6,
        Silenced = 1 << 7,
        Pacified = 1 << 8,
        Stunned = 1 << 9,
        InCombat = 1 << 10,
        Disarmed = 1 << 11,
        Confused = 1 << 12,
        Fleeing = 1 << 13,
        PlayerControlled = 1 << 14,
        NotSelectable = 1 << 15,
    }
}
