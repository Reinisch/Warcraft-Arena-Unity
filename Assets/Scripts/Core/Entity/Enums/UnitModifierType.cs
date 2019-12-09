using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    public enum UnitModifierType
    {
        Strength = StartStats + StatType.Strength,
        Agility = StartStats + StatType.Agility,
        Stamina = StartStats + StatType.Stamina,
        Intellect = StartStats + StatType.Intellect,

        Mana = StartPowers + SpellPowerType.Mana,
        Rage = StartPowers + SpellPowerType.Rage,
        Focus = StartPowers + SpellPowerType.Focus,
        Energy = StartPowers + SpellPowerType.Energy,
        ComboPoints = StartPowers + SpellPowerType.ComboPoints,
        Runes = StartPowers + SpellPowerType.Runes,
        RunicPower = StartPowers + SpellPowerType.RunicPower,
        SoulShards = StartPowers + SpellPowerType.SoulShards,
        LunarPower = StartPowers + SpellPowerType.LunarPower,
        HolyPower = StartPowers + SpellPowerType.HolyPower,
        AlternatePower = StartPowers + SpellPowerType.AlternatePower,
        Maelstrom = StartPowers + SpellPowerType.Maelstrom,
        Chi = StartPowers + SpellPowerType.Chi,
        Insanity = StartPowers + SpellPowerType.Insanity,
        BurningEmbers = StartPowers + SpellPowerType.BurningEmbers,
        DemonicFury = StartPowers + SpellPowerType.DemonicFury,
        ArcaneCharges = StartPowers + SpellPowerType.ArcaneCharges,
        Fury = StartPowers + SpellPowerType.Fury,
        Pain = StartPowers + SpellPowerType.Pain,
        Health = StartPowers + SpellPowerType.Health,

        [UsedImplicitly, HideInInspector]
        StartStats = 0,
        [UsedImplicitly, HideInInspector]
        MinStats = Strength,
        [UsedImplicitly, HideInInspector]
        MaxStats = Intellect,

        [UsedImplicitly, HideInInspector]
        StartPowers = MaxStats + 1,
        [UsedImplicitly, HideInInspector]
        MinPowers = Mana,
        [UsedImplicitly, HideInInspector]
        MaxPowers = Health,
    }
}