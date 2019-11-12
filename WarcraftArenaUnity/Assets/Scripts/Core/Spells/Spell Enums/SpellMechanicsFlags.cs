using System;

namespace Core
{
    [Flags]
    public enum SpellMechanicsFlags
    {
        Charm = 1 << 0,
        Disoriented = 1 << 1,
        Disarm = 1 << 2,
        Distract = 1 << 3,
        Fear = 1 << 4,
        Grip = 1 << 5,
        Root = 1 << 6,
        SlowAttack = 1 << 7,
        Silence = 1 << 8,
        Sleep = 1 << 9,
        Snare = 1 << 10,
        Stun = 1 << 11,
        Freeze = 1 << 12,
        Knockout = 1 << 13,
        Bleed = 1 << 14,
        Bandage = 1 << 15,
        Polymorph = 1 << 16,
        Banish = 1 << 17,
        Shield = 1 << 18,
        Shackle = 1 << 19,
        Mount = 1 << 20,
        Infected = 1 << 21,
        Horror = 1 << 22,
        Invulnerability = 1 << 23,
        Interrupt = 1 << 24,
        Daze = 1 << 25,
        ImmuneShield = 1 << 26,
        Sapped = 1 << 27,
        Enraged = 1 << 28,
        Wounded = 1 << 29
    }
}
