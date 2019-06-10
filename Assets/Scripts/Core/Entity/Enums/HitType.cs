namespace Core
{
    public enum HitType
    {
        NormalSwing = 0x00000000,
        AffectsVictim = 0x00000001,
        Offhand = 0x00000002,
        Miss = 0x00000004,
        FullAbsorb = 0x00000008,
        PartialAbsorb = 0x00000010,
        CriticalHit = 0x00000020,
        RageGain = 0x00000040,
    }
}
