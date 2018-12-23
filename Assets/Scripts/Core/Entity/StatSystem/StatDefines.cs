namespace Core
{
    public enum Stats
    {
        Strength,
        Agility,
        Stamina,
        Intellect,
    }

    public enum UnitMods
    {
        StatStrength,
        StatAgility,
        StatStamina,
        StatIntellect,
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
    
        UnitModsEnd,

        StatStart = StatStrength,
        StatEnd = StatIntellect + 1,
        ResistanceStart = Armor,
        ResistanceEnd = ResistanceArcane + 1,
        PowerStart = Mana,
        PowerEnd = Pain + 1
    }

    public enum BaseModGroup
    {
        CritPercentage,
        RangedCritPercentage,
        OffhandCritPercentage,
        ShieldBlockValue,
    }

    public enum BaseModType
    {
        FlatMod,
        PercentMod,
    }

    public enum UnitModifierType
    {
        BaseValue = 0,
        BasePercentExcludeCreate = 1,
        BasePercent = 2,
        TotalValue = 3,
        TotalPercent = 4,
        ModifierTypeEnd = 5
    }
}