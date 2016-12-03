using UnityEngine;
using System.Collections.Generic;

public static class StatSystem
{
    public static void InitializeStat(Dictionary<StatModifierType, float> modifiers)
    {
        modifiers.Add(StatModifierType.Base, 0.0f);
        modifiers.Add(StatModifierType.BaseMultExternal, 1.0f);
        modifiers.Add(StatModifierType.BaseMult, 1.0f);
        modifiers.Add(StatModifierType.Total, 0.0f);
        modifiers.Add(StatModifierType.TotalMult, 0.0f);
    }
}
