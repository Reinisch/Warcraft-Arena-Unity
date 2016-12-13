using UnityEngine;
using System.Collections.Generic;

public class Stat
{
    Dictionary<StatModifierType, float> modifiers = new Dictionary<StatModifierType, float>();

    bool isCurrent;
    int finalValue;

    public int FinalValue
    {
        get
        {
            if (isCurrent)
                return finalValue;
            else
            {
                isCurrent = true;
                float result = modifiers[StatModifierType.Base] * modifiers[StatModifierType.BaseMultExternal];
                result *= modifiers[StatModifierType.BaseMult];
                result += modifiers[StatModifierType.Total];
                result *= modifiers[StatModifierType.TotalMult];

                finalValue = (int)result;

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

    public Stat(int initialValue = 0)
    {
        StatHelper.InitializeStat(modifiers, initialValue);
    }
}