namespace Core
{
    public static class SpellUtils
    {
        public static SpellCastTargetFlags TargetFlags(this SpellTargetEntities targetEntities)
        {
            switch (targetEntities)
            {
                case SpellTargetEntities.Dest:
                    return SpellCastTargetFlags.DestLocation;
                case SpellTargetEntities.UnitAndDest:
                    return SpellCastTargetFlags.DestLocation | SpellCastTargetFlags.Unit;
                case SpellTargetEntities.Unit:
                    return SpellCastTargetFlags.Unit;
                case SpellTargetEntities.GameEntity:
                    return SpellCastTargetFlags.GameEntity;
                case SpellTargetEntities.Source:
                    return SpellCastTargetFlags.SourceLocation;
                default:
                    return 0;
            }
        }

        public static bool HasTargetFlag(this SpellInterruptFlags baseFlags, SpellInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this AuraInterruptFlags baseFlags, AuraInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellSchoolMask baseFlags, SpellSchoolMask flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellCastFlags baseFlags, SpellCastFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellRangeFlags baseFlags, SpellRangeFlags flag)
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