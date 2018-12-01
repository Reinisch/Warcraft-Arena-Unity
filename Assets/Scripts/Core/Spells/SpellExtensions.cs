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

        public static bool HasFlag(this SpellInterruptFlags baseFlags, SpellInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellAuraInterruptFlags baseFlags, SpellAuraInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellChannelInterruptFlags baseFlags, SpellChannelInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellSchoolMask baseFlags, SpellSchoolMask flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this TriggerCastFlags baseFlags, TriggerCastFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellRangeFlag baseFlags, SpellRangeFlag flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
        {
            return (baseFlags & flag) != 0;
        }
    }
}