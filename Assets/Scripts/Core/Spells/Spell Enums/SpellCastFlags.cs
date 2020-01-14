using System;

namespace Core
{
    [Flags]
    public enum SpellCastFlags
    {
        IgnoreGcd = 1 << 0,
        IgnoreSpellAndCategoryCd = 1 << 1,
        IgnorePowerAndReagentCost = 1 << 2,
        IgnoreCastItem = 1 << 3,
        IgnoreAuraScaling = 1 << 4,
        IgnoreCastInProgress = 1 << 5,
        IgnoreComboPoints = 1 << 6,
        CastDirectly = 1 << 7,
        IgnoreAuraInterruptFlags = 1 << 8,
        IgnoreSetFacing = 1 << 9,
        IgnoreShapeShift = 1 << 10,
        IgnoreCasterState = 1 << 11,
        IgnoreCasterMountedOrOnVehicle = 1 << 12,
        IgnoreCasterAuras = 1 << 13,
        DontReportCastSuccess = 1 << 14,
        DontReportCastError = 1 << 15,
        IgnoreRangeCheck = 1 << 16,
        IgnoreTargetCheck = 1 << 17,
        TriggeredByAura = 1 << 18
    }
}
