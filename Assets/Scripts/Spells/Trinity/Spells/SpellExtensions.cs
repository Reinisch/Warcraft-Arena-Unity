using UnityEngine;
using System.Collections;

public static class SpellExtensions
{
    public static bool HasFlag(this TriggerCastFlags baseFlags, TriggerCastFlags flag)
    {
        return (baseFlags & flag) == flag;
    }
    public static bool HasFlag(this SpellPreventionType baseFlags, SpellPreventionType flag)
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
    public static bool HasFlag(this TargetTypes baseFlags, TargetTypes flag)
    {
        return (baseFlags & flag) == flag;
    }

    public static SpellTargetObjectTypes ObjectType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].ObjectType;
    }
    public static SpellTargetReferenceTypes ReferenceType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].ReferenceType;
    }
    public static SpellTargetSelectionCategories Category(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].SelectionCategory;
    }
    public static SpellTargetCheckTypes CheckType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].SelectionCheckType;
    }
    public static SpellTargetDirectionTypes DirectionType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].DirectionType;
    }


    private struct StaticTargetData
    {
        public SpellTargetObjectTypes ObjectType;    // type of object returned by target type
        public SpellTargetReferenceTypes ReferenceType; // defines which object is used as a reference when selecting target
        public SpellTargetSelectionCategories SelectionCategory;
        public SpellTargetCheckTypes SelectionCheckType; // defines selection criteria
        public SpellTargetDirectionTypes DirectionType; // direction for cone and dest targets
    };
    private static StaticTargetData[] targetTypeData = new StaticTargetData[(int)TargetTypes.TOTAL_SPELL_TARGETS];
}