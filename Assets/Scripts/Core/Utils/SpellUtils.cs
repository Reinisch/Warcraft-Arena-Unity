namespace Core
{
    public static class SpellUtils
    {
        public static bool HasTargetFlag(this SpellInterruptFlags baseFlags, SpellInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this AuraInterruptFlags baseFlags, AuraInterruptFlags flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this AuraInterruptFlags baseFlags, AuraInterruptFlags flag)
        {
            return (baseFlags & flag) != 0;
        }

        public static bool HasTargetFlag(this SpellSchoolMask baseFlags, SpellSchoolMask flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this HitType baseFlags, HitType flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasTargetFlag(this SpellPreventionType baseFlags, SpellPreventionType flag)
        {
            return (baseFlags & flag) == flag;
        }

        public static bool HasAnyFlag(this SpellPreventionType baseFlags, SpellPreventionType flag)
        {
            return (baseFlags & flag) != 0;
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

        public static bool IsRemoved(this AuraRemoveMode mode)
        {
            return mode != AuraRemoveMode.None;
        }
    }
}