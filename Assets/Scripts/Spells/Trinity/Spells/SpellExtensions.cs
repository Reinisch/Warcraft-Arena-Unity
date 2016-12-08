using UnityEngine;
using System.Collections;

public static class SpellExtensions
{
    public static bool HasFlag(this TriggerCastFlags baseFlags, TriggerCastFlags flag)
    {
        return (baseFlags & flag) == flag;
    }
}