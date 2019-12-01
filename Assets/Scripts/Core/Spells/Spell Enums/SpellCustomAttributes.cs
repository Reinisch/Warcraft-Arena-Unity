using System;

namespace Core
{
    [Flags]
    public enum SpellCustomAttributes
    {
        EnchantProc = 1 << 0,
        ConeBack = 1 << 1,
        ConeLine = 1 << 2,
        ShareDamage = 1 << 3,
        NoInitialThreat = 1 << 4,
        IsTalent = 1 << 5,
        DontBreakStealth = 1 << 6,
        DirectDamage = 1 << 7,
        Charge = 1 << 8,
        Pickpocket = 1 << 9,
        Negative = 1 << 10,
        CastWithoutAnimation = 1 << 11,
        ReqTargetFacingCaster = 1 << 12,
        ReqCasterBehindTarget = 1 << 13,
        AllowInFlightTarget = 1 << 14,
        LaunchSourceIsExplicit = 1 << 15,
    }
}
