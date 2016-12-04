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

public static class StatSystem
{
    #region Static info

    private static Dictionary<UnitMoveType, float> baseMoveSpeed = new Dictionary<UnitMoveType, float>()
    {
        { UnitMoveType.Run, 7.0f },
        { UnitMoveType.RunBack, 4.5f },
        { UnitMoveType.TurnRate, 3.14f },
        { UnitMoveType.PitchRate, 3.14f },
    };

    public static List<StatType> UnitStats = new List<StatType>()
    {
        StatType.Strength,
        StatType.Agility,
        StatType.Stamina,
        StatType.Intellect,
        StatType.Health,
        StatType.Mana,
        StatType.Rage,
        StatType.Focus,
        StatType.Energy,
        StatType.Rune,
        StatType.RunicPower,
        StatType.SoulShards,
        StatType.Eclipse,
        StatType.HolyPower,
        StatType.Alternative,
        StatType.Maelstrom,
        StatType.Chi,
        StatType.Insanity,
        StatType.BurningEmbers,
        StatType.DemonicFury,
        StatType.ArcaneCharges,
        StatType.Fury,
        StatType.Pain,
        StatType.Armor,
        StatType.ResistanceHoly,
        StatType.ResistanceFire,
        StatType.ResistanceNature,
        StatType.ResistanceFrost,
        StatType.ResistanceShadow,
        StatType.ResistanceArcane,
        StatType.AttackPower,
        StatType.AttackPowerRanged,
        StatType.DamageMainhand,
        StatType.DamageOffhand,
        StatType.DamageRanged,
    };

    #endregion


    public static void InitializeStat(Dictionary<StatModifierType, float> modifiers, float initialValue = 0.0f)
    {
        modifiers.Clear();

        modifiers.Add(StatModifierType.Base, initialValue);
        modifiers.Add(StatModifierType.BaseMultExternal, 1.0f);
        modifiers.Add(StatModifierType.BaseMult, 1.0f);
        modifiers.Add(StatModifierType.Total, 0.0f);
        modifiers.Add(StatModifierType.TotalMult, 0.0f);
    }

    public static void InitializePlayerStats(Dictionary<StatType, Stat> playerStats)
    {
        playerStats.Clear();

        playerStats.Add(StatType.Strength, new Stat(28));
        playerStats.Add(StatType.Agility, new Stat(22));
        playerStats.Add(StatType.Stamina, new Stat(30));
        playerStats.Add(StatType.Intellect, new Stat(32));
        playerStats.Add(StatType.Health, new Stat(1000));
        playerStats.Add(StatType.Mana, new Stat(100));
        playerStats.Add(StatType.Rage, new Stat(100));
        playerStats.Add(StatType.Focus, new Stat(100));
        playerStats.Add(StatType.Energy, new Stat(100));
        playerStats.Add(StatType.Rune, new Stat(100));
        playerStats.Add(StatType.RunicPower, new Stat(100));
        playerStats.Add(StatType.SoulShards, new Stat(100));
        playerStats.Add(StatType.Eclipse, new Stat(100));
        playerStats.Add(StatType.HolyPower, new Stat(100));
        playerStats.Add(StatType.Alternative, new Stat(100));
        playerStats.Add(StatType.Maelstrom, new Stat(100));
        playerStats.Add(StatType.Chi, new Stat(100));
        playerStats.Add(StatType.Insanity, new Stat(100));
        playerStats.Add(StatType.BurningEmbers, new Stat(5));
        playerStats.Add(StatType.DemonicFury, new Stat(5));
        playerStats.Add(StatType.ArcaneCharges, new Stat(5));
        playerStats.Add(StatType.Fury, new Stat(100));
        playerStats.Add(StatType.Pain, new Stat(100));
        playerStats.Add(StatType.Armor, new Stat(20));
        playerStats.Add(StatType.ResistanceHoly, new Stat(0));
        playerStats.Add(StatType.ResistanceFire, new Stat(0));
        playerStats.Add(StatType.ResistanceNature, new Stat(0));
        playerStats.Add(StatType.ResistanceFrost, new Stat(0));
        playerStats.Add(StatType.ResistanceShadow, new Stat(0));
        playerStats.Add(StatType.ResistanceArcane, new Stat(0));
        playerStats.Add(StatType.AttackPower, new Stat(20));
        playerStats.Add(StatType.AttackPowerRanged, new Stat(20));
        playerStats.Add(StatType.DamageMainhand, new Stat(1));
        playerStats.Add(StatType.DamageOffhand, new Stat(1));
        playerStats.Add(StatType.DamageRanged, new Stat(1));

        if (playerStats.Count != unitStats.Count)
            Debug.LogError("Wrong player stats initialization count!");
    }

    public static void InitializeCreatureStats(Dictionary<StatType, Stat> creatureStats)
    {
        creatureStats.Clear();

        creatureStats.Add(StatType.Strength, new Stat(0));
        creatureStats.Add(StatType.Agility, new Stat(0));
        creatureStats.Add(StatType.Stamina, new Stat(0));
        creatureStats.Add(StatType.Intellect, new Stat(0));
        creatureStats.Add(StatType.Health, new Stat(1000));
        creatureStats.Add(StatType.Mana, new Stat(100));
        creatureStats.Add(StatType.Rage, new Stat(100));
        creatureStats.Add(StatType.Focus, new Stat(100));
        creatureStats.Add(StatType.Energy, new Stat(100));
        creatureStats.Add(StatType.Rune, new Stat(100));
        creatureStats.Add(StatType.RunicPower, new Stat(100));
        creatureStats.Add(StatType.SoulShards, new Stat(100));
        creatureStats.Add(StatType.Eclipse, new Stat(100));
        creatureStats.Add(StatType.HolyPower, new Stat(100));
        creatureStats.Add(StatType.Alternative, new Stat(100));
        creatureStats.Add(StatType.Maelstrom, new Stat(100));
        creatureStats.Add(StatType.Chi, new Stat(100));
        creatureStats.Add(StatType.Insanity, new Stat(100));
        creatureStats.Add(StatType.BurningEmbers, new Stat(0));
        creatureStats.Add(StatType.DemonicFury, new Stat(0));
        creatureStats.Add(StatType.ArcaneCharges, new Stat(0));
        creatureStats.Add(StatType.Fury, new Stat(100));
        creatureStats.Add(StatType.Pain, new Stat(100));
        creatureStats.Add(StatType.Armor, new Stat(0));
        creatureStats.Add(StatType.ResistanceHoly, new Stat(0));
        creatureStats.Add(StatType.ResistanceFire, new Stat(0));
        creatureStats.Add(StatType.ResistanceNature, new Stat(0));
        creatureStats.Add(StatType.ResistanceFrost, new Stat(0));
        creatureStats.Add(StatType.ResistanceShadow, new Stat(0));
        creatureStats.Add(StatType.ResistanceArcane, new Stat(0));
        creatureStats.Add(StatType.AttackPower, new Stat(0));
        creatureStats.Add(StatType.AttackPowerRanged, new Stat(0));
        creatureStats.Add(StatType.DamageMainhand, new Stat(1));
        creatureStats.Add(StatType.DamageOffhand, new Stat(1));
        creatureStats.Add(StatType.DamageRanged, new Stat(1));

        if (creatureStats.Count != unitStats.Count)
            Debug.LogError("Wrong player stats initialization count!");
    }

    public static void InitializeSpeedRates(Dictionary<UnitMoveType, float> speedRates)
    {
        speedRates.Clear();

        foreach (var speedRate in baseMoveSpeed)
            speedRates.Add(speedRate.Key, 1.0f);
    }

    public static float BaseMovementSpeed(UnitMoveType moveType)
    {
        return baseMoveSpeed[moveType];
    }

}
