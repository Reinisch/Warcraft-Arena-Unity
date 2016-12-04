using UnityEngine;
using System.Collections.Generic;

public class Stat
{
    Dictionary<StatModifierType, float> modifiers = new Dictionary<StatModifierType, float>();

    bool isCurrent;
    float finalValue;

    public float FinalValue
    {
        get
        {
            if (isCurrent)
                return finalValue;
            else
            {
                isCurrent = true;
                finalValue = modifiers[StatModifierType.Base] * modifiers[StatModifierType.BaseMultExternal];
                finalValue *= modifiers[StatModifierType.BaseMult];
                finalValue += modifiers[StatModifierType.Total];
                finalValue *= modifiers[StatModifierType.TotalMult];

                if (finalValue < 0)
                    finalValue = 0;

                return finalValue;
            }
        }
    }

    public float this[StatModifierType modType]
    {
        get
        {
            return modifiers[modType];
        }
        set
        {
            modifiers[modType] = value;
            isCurrent = false;
        }
    }

    public Stat(float initialValue = 0.0f)
    {
        StatHelper.InitializeStat(modifiers, initialValue);
    }
}