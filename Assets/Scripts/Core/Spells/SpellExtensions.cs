namespace Core
{
    public static class SpellExtensions
    {
        public static SpellCastTargetFlags TargetFlags(this TargetEntities targetEntities)
        {
            switch (targetEntities)
            {
                case TargetEntities.Dest:
                    return SpellCastTargetFlags.DestLocation;
                case TargetEntities.UnitAndDest:
                    return SpellCastTargetFlags.DestLocation | SpellCastTargetFlags.Unit;
                case TargetEntities.Unit:
                    return SpellCastTargetFlags.Unit;
                case TargetEntities.GameEntity:
                    return SpellCastTargetFlags.GameEntity;
                case TargetEntities.Source:
                    return SpellCastTargetFlags.SourceLocation;
                default:
                    return 0;
            }
        }

        public static bool HasTargetFlag(this SpellInterruptFlags baseFlags, SpellInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellAuraInterruptFlags baseFlags, SpellAuraInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellChannelInterruptFlags baseFlags, SpellChannelInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellSchoolMask baseFlags, SpellSchoolMask flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this TriggerCastFlags baseFlags, TriggerCastFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellRangeFlag baseFlags, SpellRangeFlag flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
        {
            return (baseFlags & flag) != 0;
        }
    }
}