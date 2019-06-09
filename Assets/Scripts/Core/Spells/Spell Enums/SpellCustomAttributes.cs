using System;

namespace Core
{
    [Flags]
    public enum SpellCustomAttributes
    {
        EnchantProc = 0x00000001,
        ConeBack = 0x00000002,
        ConeLine = 0x00000004,
        ShareDamage = 0x00000008,
        NoInitialThreat = 0x00000010,
        IsTalent = 0x00000020,
        DontBreakStealth = 0x00000040,
        DirectDamage = 0x00000100,
        Charge = 0x00000200,
        Pickpocket = 0x00000400,
        Negative = 0x00001000,
        IgnoreArmor = 0x00008000,
        ReqTargetFacingCaster = 0x00010000,
        ReqCasterBehindTarget = 0x00020000,
        AllowInflightTarget = 0x00040000,
        NeedsAmmoData = 0x00080000,
    }
}
