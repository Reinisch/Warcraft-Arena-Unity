using UnityEngine;
using System.Collections.Generic;

public enum BaseStatType
{
    Strength,
    Agility,
    Stamina,
    Intellect,
}

public enum StatType
{
    Strength,
    Agility,
    Stamina,
    Intellect,

    Health,

    Mana,
    Rage,
    Focus,
    Energy,
    Rune,
    RunicPower,
    SoulShards,
    Eclipse,
    HolyPower,
    Alternative,
    Maelstrom,
    Chi,
    Insanity,
    BurningEmbers,
    DemonicFury,
    ArcaneCharges,
    Fury,
    Pain,

    Armor,
    ResistanceHoly,
    ResistanceFire,
    ResistanceNature,
    ResistanceFrost,
    ResistanceShadow,
    ResistanceArcane,

    AttackPower,
    AttackPowerRanged,
    DamageMainhand,
    DamageOffhand,
    DamageRanged,

    StatCount
};

public enum StatModifierType
{
    Base,
    BaseMultExternal,
    BaseMult,
    Total,
    TotalMult,

    StatModifierCount
};

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

    public Stat()
    {
        StatSystem.InitializeStat(modifiers);
    }
}

