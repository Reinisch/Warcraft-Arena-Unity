using System;

namespace Core
{
    public enum CorpseType
    {
        Bones = 0,
        ResurrectablePve = 1,
        ResurrectablePvp = 2
    }

    [Flags]
    public enum CorpseFlags
    {
        None = 0x00,
        Bones = 0x01,
        HideHelm = 0x08,
        HideCloak = 0x10,
        Lootable = 0x20
    }
}