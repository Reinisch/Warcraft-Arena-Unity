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
}