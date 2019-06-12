using System;

namespace Core
{
    [Flags]
    public enum FieldFlags
    {
        None = 0,
        Public = 1 << 0,
        Private = 1 << 1,
        Owner = 1 << 2,
        ItemOwner = 1 << 3,
        SpecialInfo = 1 << 4,
        PartyMember = 1 << 5,
        UnitAll = 1 << 6,
        Dynamic = 1 << 7,
        Urgent = 1 << 8,
        UrgentSelfOnly = 1 << 9,
    }

    public enum FieldTypes
    {
        Int,
        Uint,
        Ulong,
        Long,
        Float,
        Double,
        Short,
        UShort
    }

    public enum EntityFields
    {
        /// <summary> Type: int, Flags: DYNAMIC </summary>
        Entry = BaseEntityFields.Entry,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        Scale = BaseEntityFields.Scale,
        /// <summary> Type: long, Flags: DYNAMIC, URGENT </summary>
        DynamicFlags = BaseEntityFields.DynamicFlags,

        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        GameEntityCreatedBy = GameEntityFields.CreatedBy,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        GameEntityDisplayId = GameEntityFields.DisplayId,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        GameEntityFlags = GameEntityFields.GameEntityFlags,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation0 = GameEntityFields.ParentRotation0,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation1 = GameEntityFields.ParentRotation1,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation2 = GameEntityFields.ParentRotation2,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation3 = GameEntityFields.ParentRotation3,
        /// <summary> Type: uint, Flags: PUBLIC </summary>
        GameEntityFaction = GameEntityFields.Faction,
        /// <summary> Type: uint, Flags: PUBLIC </summary>
        GameEntityLevel = GameEntityFields.GameEntityLevel,
        /// <summary> Type: uint, Flags: PUBLIC, URGENT </summary>
        GameEntityInfo = GameEntityFields.GameEntityInfo,
        /// <summary> Type: uint, Flags: PUBLIC, DYNAMIC, URGENT </summary>
        SpellVisualID = GameEntityFields.SpellVisualID,
        /// <summary> Type: uint, Flags: PUBLIC, DYNAMIC, URGENT </summary>
        StateSpellVisualID = GameEntityFields.StateSpellVisualID,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        StateAnimID = GameEntityFields.StateAnimID,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        StateAnimKitID = GameEntityFields.StateAnimKitID,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        StateWorldEffectID = GameEntityFields.StateWorldEffectID,

        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        DynamicCaster = DynamicEntityFields.DynamicCaster,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicEntityType = DynamicEntityFields.DynamicEntityType,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicSpellVisualID = DynamicEntityFields.DynamicSpellVisualID,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicSpellid = DynamicEntityFields.DynamicSpellid,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        DynamicRadius = DynamicEntityFields.DynamicRadius,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicCasttime = DynamicEntityFields.DynamicCastTime,

        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        UnitCharm = UnitFields.Charm,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        UnitSummon = UnitFields.Summon,
        /// <summary> Type: NetworkId, Flags: PRIVATE </summary>
        UnitCritter = UnitFields.Critter,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        UnitCharmedBy = UnitFields.CharmedBy,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        UnitSummonedBy = UnitFields.SummonedBy,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        UnitCreatedBy = UnitFields.CreatedBy,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        DemonCreator = UnitFields.DemonCreator,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        Target = UnitFields.Target,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        BattlePetCompanionGuid = UnitFields.BattlePetCompanionGuid,
        /// <summary> Type: NetworkId, Flags: PUBLIC, URGENT </summary>
        ChannelObject = UnitFields.ChannelObject,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        ChannelSpell = UnitFields.ChannelSpell,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        ChannelSpellVisual = UnitFields.ChannelSpellVisual,
        /// <summary> Type: Int - Bytes: 1 byte race, 1 byte class, 1 byte gender, Flags: PUBLIC </summary>
        Info = UnitFields.Info,
        /// <summary> Type: Int - Powers, Flags: PUBLIC </summary>
        DisplayPower = UnitFields.DisplayPower,
        /// <summary> Type: Int - Powers, Flags: PUBLIC </summary>
        OverrideDisplayPowerId = UnitFields.OverrideDisplayPowerId,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        Health = UnitFields.Health,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power = UnitFields.Power,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power1 = UnitFields.Power1,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power2 = UnitFields.Power2,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power3 = UnitFields.Power3,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power4 = UnitFields.Power4,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power5 = UnitFields.Power5,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxHealth = UnitFields.MaxHealth,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower = UnitFields.MaxPower,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower1 = UnitFields.MaxPower1,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower2 = UnitFields.MaxPower2,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower3 = UnitFields.MaxPower3,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower4 = UnitFields.MaxPower4,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower5 = UnitFields.MaxPower5,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier = UnitFields.PowerRegenFlatModifier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier1 = UnitFields.PowerRegenFlatModifier1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier2 = UnitFields.PowerRegenFlatModifier2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier3 = UnitFields.PowerRegenFlatModifier3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier4 = UnitFields.PowerRegenFlatModifier4,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier5 = UnitFields.PowerRegenFlatModifier5,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier = UnitFields.PowerRegenInterruptedFlatModifier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier1 = UnitFields.PowerRegenInterruptedFlatModifier1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier2 = UnitFields.PowerRegenInterruptedFlatModifier2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier3 = UnitFields.PowerRegenInterruptedFlatModifier3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier4 = UnitFields.PowerRegenInterruptedFlatModifier4,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier5 = UnitFields.PowerRegenInterruptedFlatModifier5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        Level = UnitFields.Level,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        FactionTemplate = UnitFields.FactionTemplate,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId = UnitFields.UnitVirtualItemSlotId,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId1 = UnitFields.UnitVirtualItemSlotId1,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId2 = UnitFields.UnitVirtualItemSlotId2,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId3 = UnitFields.UnitVirtualItemSlotId3,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId4 = UnitFields.UnitVirtualItemSlotId4,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId5 = UnitFields.UnitVirtualItemSlotId5,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT </summary>
        UnitFlags = UnitFields.UnitFlags,
        /// <summary> Type: Int - AuraStateType, Flags: PUBLIC </summary>
        AuraState = UnitFields.AuraState,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseAttackTimeMain = UnitFields.BaseAttackTimeMain,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseAttackTimeOffhand = UnitFields.BaseAttackTimeOffhand,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseAttackTimeRanged = UnitFields.BaseAttackTimeRanged,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        BoundingRadius = UnitFields.BoundingRadius,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        CombatReach = UnitFields.CombatReach,
        /// <summary> Type: Uint, Flags: DYNAMIC, URGENT </summary>
        DisplayId = UnitFields.DisplayId,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        NativeDisplayId = UnitFields.NativeDisplayId,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        MountDisplayId = UnitFields.MountDisplayId,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MinDamage = UnitFields.MinDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MaxDamage = UnitFields.MaxDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MinOffhandDamage = UnitFields.MinOffhandDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MaxOffhandDamage = UnitFields.MaxOffhandDamage,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        PetNumber = UnitFields.PetNumber,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        PetNameTimestamp = UnitFields.PetNameTimestamp,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        PetExperience = UnitFields.PetExperience,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        PetNextLevelExp = UnitFields.PetNextLevelExp,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        UnitModCastSpeed = UnitFields.UnitModCastSpeed,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        UnitModCastHaste = UnitFields.UnitModCastHaste,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModHaste = UnitFields.ModHaste,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModRangedHaste = UnitFields.ModRangedHaste,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModHasteRegen = UnitFields.ModHasteRegen,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModTimeRate = UnitFields.ModTimeRate,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        UnitCreatedBySpell = UnitFields.UnitCreatedBySpell,
        /// <summary> Type: Long - NpcFlags, Flags: PUBLIC, DYNAMIC </summary>
        UnitNpcFlags = UnitFields.UnitNpcFlags,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat = UnitFields.Stat,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat1 = UnitFields.Stat1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat2 = UnitFields.Stat2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat3 = UnitFields.Stat3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat = UnitFields.PosStat,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat1 = UnitFields.PosStat1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat2 = UnitFields.PosStat2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat3 = UnitFields.PosStat3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat = UnitFields.NegStat,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat1 = UnitFields.NegStat1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat2 = UnitFields.NegStat2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat3 = UnitFields.NegStat3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances = UnitFields.Resistances,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances1 = UnitFields.Resistances1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances2 = UnitFields.Resistances2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances3 = UnitFields.Resistances3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances4 = UnitFields.Resistances4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances5 = UnitFields.Resistances5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances6 = UnitFields.Resistances6,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive = UnitFields.ResistanceBuffModsPositive,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive1 = UnitFields.ResistanceBuffModsPositive1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive2 = UnitFields.ResistanceBuffModsPositive2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive3 = UnitFields.ResistanceBuffModsPositive3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive4 = UnitFields.ResistanceBuffModsPositive4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive5 = UnitFields.ResistanceBuffModsPositive5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive6 = UnitFields.ResistanceBuffModsPositive6,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative = UnitFields.ResistanceBuffModsNegative,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative1 = UnitFields.ResistanceBuffModsNegative1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative2 = UnitFields.ResistanceBuffModsNegative2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative3 = UnitFields.ResistanceBuffModsNegative3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative4 = UnitFields.ResistanceBuffModsNegative4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative5 = UnitFields.ResistanceBuffModsNegative5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative6 = UnitFields.ResistanceBuffModsNegative6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseMana = UnitFields.BaseMana,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        BaseHealth = UnitFields.BaseHealth,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseFlags = UnitFields.BaseFlags,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        AttackPower = UnitFields.AttackPower,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        AttackPowerModPos = UnitFields.AttackPowerModPos,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        AttackPowerModNeg = UnitFields.AttackPowerModNeg,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        AttackPowerMultiplier = UnitFields.AttackPowerMultiplier,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        RangedAttackPower = UnitFields.RangedAttackPower,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        RangedAttackPowerModPos = UnitFields.RangedAttackPowerModPos,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        RangedAttackPowerModNeg = UnitFields.RangedAttackPowerModNeg,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        RangedAttackPowerMultiplier = UnitFields.RangedAttackPowerMultiplier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        MinRangedDamage = UnitFields.MinRangedDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        MaxRangedDamage = UnitFields.MaxRangedDamage,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier = UnitFields.PowerCostModifier,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier1 = UnitFields.PowerCostModifier1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier2 = UnitFields.PowerCostModifier2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier3 = UnitFields.PowerCostModifier3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier4 = UnitFields.PowerCostModifier4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier5 = UnitFields.PowerCostModifier5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier6 = UnitFields.PowerCostModifier6,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier = UnitFields.PowerCostMultiplier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier1 = UnitFields.PowerCostMultiplier1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier2 = UnitFields.PowerCostMultiplier2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier3 = UnitFields.PowerCostMultiplier3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier4 = UnitFields.PowerCostMultiplier4,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier5 = UnitFields.PowerCostMultiplier5,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier6 = UnitFields.PowerCostMultiplier6,

        /// <summary> Type: Long - PlayerFlags, Flags: PUBLIC </summary>
        PlayerFlags = PlayerFields.PlayerFlags,
        /// <summary> Type: Int - ArenaTeam, Flags: PUBLIC </summary>
        ArenaTeam = PlayerFields.ArenaTeam,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DuelTeam = PlayerFields.DuelTeam,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ChosenTitle = PlayerFields.ChosenTitle,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CurrentSpecId = PlayerFields.CurrentSpecId,
        /// <summary> Type: Long, Flags: PRIVATE </summary>
        Coinage = PlayerFields.Coinage,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        Xp = PlayerFields.Xp,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        PlayerCharacterPoints = PlayerFields.PlayerCharacterPoints,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        NextLevelXp = PlayerFields.NextLevelXp,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        BlockPercentage = PlayerFields.BlockPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        DodgePercentage = PlayerFields.DodgePercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        DodgePercentageFromAttribute = PlayerFields.DodgePercentageFromAttribute,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ParryPercentage = PlayerFields.ParryPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ParryPercentageFromAttribute = PlayerFields.ParryPercentageFromAttribute,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        CritPercentage = PlayerFields.CritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        RangedCritPercentage = PlayerFields.RangedCritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        OffhandCritPercentage = PlayerFields.OffhandCritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        SpellCritPercentage = PlayerFields.SpellCritPercentage,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ShieldBlock = PlayerFields.ShieldBlock,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ShieldBlockCritPercentage = PlayerFields.ShieldBlockCritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        Mastery = PlayerFields.Mastery,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos = PlayerFields.ModDamageDonePos,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos1 = PlayerFields.ModDamageDonePos1,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos2 = PlayerFields.ModDamageDonePos2,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos3 = PlayerFields.ModDamageDonePos3,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos4 = PlayerFields.ModDamageDonePos4,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos5 = PlayerFields.ModDamageDonePos5,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos6 = PlayerFields.ModDamageDonePos6,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg = PlayerFields.ModDamageDoneNeg,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg1 = PlayerFields.ModDamageDoneNeg1,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg2 = PlayerFields.ModDamageDoneNeg2,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg3 = PlayerFields.ModDamageDoneNeg3,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg4 = PlayerFields.ModDamageDoneNeg4,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg5 = PlayerFields.ModDamageDoneNeg5,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg6 = PlayerFields.ModDamageDoneNeg6,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct = PlayerFields.ModDamageDonePct,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct1 = PlayerFields.ModDamageDonePct1,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct2 = PlayerFields.ModDamageDonePct2,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct3 = PlayerFields.ModDamageDonePct3,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct4 = PlayerFields.ModDamageDonePct4,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct5 = PlayerFields.ModDamageDonePct5,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct6 = PlayerFields.ModDamageDonePct6,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        OverrideSpellPowerByApPct = PlayerFields.OverrideSpellPowerByApPct,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        OverrideApBySpellPowerPercent = PlayerFields.OverrideApBySpellPowerPercent,
        /// <summary> Type: Int, Flags: PRIVATE </summary>
        ModTargetResistance = PlayerFields.ModTargetResistance,
        /// <summary> Type: Int, Flags: PRIVATE </summary>
        ModTargetPhysicalResistance = PlayerFields.ModTargetPhysicalResistance,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating1 = PlayerFields.CombatRating1,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating2 = PlayerFields.CombatRating2,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating3 = PlayerFields.CombatRating3,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating4 = PlayerFields.CombatRating4,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating5 = PlayerFields.CombatRating5,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating6 = PlayerFields.CombatRating6,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating7 = PlayerFields.CombatRating7,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating8 = PlayerFields.CombatRating8,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating9 = PlayerFields.CombatRating9,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating10 = PlayerFields.CombatRating10,

        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemOwner = ItemFields.ItemOwner,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemContained = ItemFields.ItemContained,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemCreator = ItemFields.ItemCreator,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemGiftcreator = ItemFields.ItemGiftcreator,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemStackCount = ItemFields.ItemStackCount,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemDuration = ItemFields.ItemDuration,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges = ItemFields.ItemSpellCharges,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges1 = ItemFields.ItemSpellCharges1,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges2 = ItemFields.ItemSpellCharges2,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges3 = ItemFields.ItemSpellCharges3,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges4 = ItemFields.ItemSpellCharges4,
        /// <summary> Type: Uint - ItemFieldFlags, Flags: PUBLIC </summary>
        ItemFlags = ItemFields.ItemFlags,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID = ItemFields.ItemEnchantmentID,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration = ItemFields.ItemEnchantmentDuration,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges = ItemFields.ItemEnchantmentCharges,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID1 = ItemFields.ItemEnchantmentID1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration1 = ItemFields.ItemEnchantmentDuration1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges1 = ItemFields.ItemEnchantmentCharges1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID2 = ItemFields.ItemEnchantmentID2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration2 = ItemFields.ItemEnchantmentDuration2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges2 = ItemFields.ItemEnchantmentCharges2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID3 = ItemFields.ItemEnchantmentID3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration3 = ItemFields.ItemEnchantmentDuration3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges3 = ItemFields.ItemEnchantmentCharges3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID4 = ItemFields.ItemEnchantmentID4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration4 = ItemFields.ItemEnchantmentDuration4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges4 = ItemFields.ItemEnchantmentCharges4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID5 = ItemFields.ItemEnchantmentID5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration5 = ItemFields.ItemEnchantmentDuration5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges5 = ItemFields.ItemEnchantmentCharges5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID6 = ItemFields.ItemEnchantmentID6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration6 = ItemFields.ItemEnchantmentDuration6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges6 = ItemFields.ItemEnchantmentCharges6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID7 = ItemFields.ItemEnchantmentID7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration7 = ItemFields.ItemEnchantmentDuration7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges7 = ItemFields.ItemEnchantmentCharges7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID8 = ItemFields.ItemEnchantmentID8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration8 = ItemFields.ItemEnchantmentDuration8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges8 = ItemFields.ItemEnchantmentCharges8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID9 = ItemFields.ItemEnchantmentID9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration9 = ItemFields.ItemEnchantmentDuration9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges9 = ItemFields.ItemEnchantmentCharges9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID10 = ItemFields.ItemEnchantmentID10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration10 = ItemFields.ItemEnchantmentDuration10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges10 = ItemFields.ItemEnchantmentCharges10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID11 = ItemFields.ItemEnchantmentID11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration11 = ItemFields.ItemEnchantmentDuration11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges11 = ItemFields.ItemEnchantmentCharges11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID12 = ItemFields.ItemEnchantmentID12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration12 = ItemFields.ItemEnchantmentDuration12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges12 = ItemFields.ItemEnchantmentCharges12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemPropertySeed = ItemFields.ItemPropertySeed,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemRandomPropertiesID = ItemFields.ItemRandomPropertiesID,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemDurability = ItemFields.ItemDurability,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemMaxdurability = ItemFields.ItemMaxdurability,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemCreatePlayedTime = ItemFields.ItemCreatePlayedTime,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemModifiersMask = ItemFields.ItemModifiersMask,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemContext = ItemFields.ItemContext,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemArtifactXP = ItemFields.ItemArtifactXP,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemAppearanceModID = ItemFields.ItemAppearanceModID,

        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot1 = ContainerFields.ContainerFieldSlot1,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot2 = ContainerFields.ContainerFieldSlot2,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot3 = ContainerFields.ContainerFieldSlot3,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot4 = ContainerFields.ContainerFieldSlot4,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot5 = ContainerFields.ContainerFieldSlot5,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot6 = ContainerFields.ContainerFieldSlot6,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot7 = ContainerFields.ContainerFieldSlot7,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot8 = ContainerFields.ContainerFieldSlot8,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot9 = ContainerFields.ContainerFieldSlot9,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot10 = ContainerFields.ContainerFieldSlot10,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot11 = ContainerFields.ContainerFieldSlot11,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot12 = ContainerFields.ContainerFieldSlot12,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot13 = ContainerFields.ContainerFieldSlot13,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot14 = ContainerFields.ContainerFieldSlot14,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot15 = ContainerFields.ContainerFieldSlot15,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot16 = ContainerFields.ContainerFieldSlot16,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot17 = ContainerFields.ContainerFieldSlot17,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot18 = ContainerFields.ContainerFieldSlot18,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot19 = ContainerFields.ContainerFieldSlot19,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot20 = ContainerFields.ContainerFieldSlot20,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot21 = ContainerFields.ContainerFieldSlot21,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot22 = ContainerFields.ContainerFieldSlot22,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot23 = ContainerFields.ContainerFieldSlot23,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot24 = ContainerFields.ContainerFieldSlot24,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot25 = ContainerFields.ContainerFieldSlot25,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot26 = ContainerFields.ContainerFieldSlot26,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot27 = ContainerFields.ContainerFieldSlot27,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot28 = ContainerFields.ContainerFieldSlot28,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot29 = ContainerFields.ContainerFieldSlot29,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot30 = ContainerFields.ContainerFieldSlot30,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot31 = ContainerFields.ContainerFieldSlot31,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot32 = ContainerFields.ContainerFieldSlot32,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot33 = ContainerFields.ContainerFieldSlot33,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot34 = ContainerFields.ContainerFieldSlot34,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot35 = ContainerFields.ContainerFieldSlot35,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot36 = ContainerFields.ContainerFieldSlot36,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ContainerFieldNumSlots = ContainerFields.ContainerFieldNumSlots,

        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        CorpseOwner = CorpseFields.CorpseOwner,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        CorpseParty = CorpseFields.CorpseParty,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseDisplayID = CorpseFields.CorpseDisplayID,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem = CorpseFields.CorpseItem,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem1 = CorpseFields.CorpseItem1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem2 = CorpseFields.CorpseItem2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem3 = CorpseFields.CorpseItem3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem4 = CorpseFields.CorpseItem4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem5 = CorpseFields.CorpseItem5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem6 = CorpseFields.CorpseItem6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem7 = CorpseFields.CorpseItem7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem8 = CorpseFields.CorpseItem8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem9 = CorpseFields.CorpseItem9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem10 = CorpseFields.CorpseItem10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem11 = CorpseFields.CorpseItem11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem12 = CorpseFields.CorpseItem12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem13 = CorpseFields.CorpseItem13,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem14 = CorpseFields.CorpseItem14,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem15 = CorpseFields.CorpseItem15,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem16 = CorpseFields.CorpseItem16,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem17 = CorpseFields.CorpseItem17,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem18 = CorpseFields.CorpseItem18,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseInfo = CorpseFields.CorpseInfo,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseExtraInfo = CorpseFields.CorpseExtraInfo,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseFlags = CorpseFields.CorpseFlags,
        /// <summary> Type: Uint, Flags: DYNAMIC </summary>
        CorpseDynamicFlags = CorpseFields.CorpseDynamicFlags,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseFactiontemplate = CorpseFields.CorpseFactiontemplate,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseCustomDisplayOption = CorpseFields.CorpseCustomDisplayOption,
    }


    public enum BaseEntityFields
    {
        /// <summary> Type: int, Flags: DYNAMIC </summary>
        Entry = 1,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        Scale = 2,
        /// <summary> Type: long, Flags: DYNAMIC, URGENT </summary>
        DynamicFlags = 3,
    }

    public enum GameEntityFields
    {
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        CreatedBy = BaseEntityFields.DynamicFlags + 1,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        DisplayId,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        GameEntityFlags,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation0,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation1,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation2,
        /// <summary> Type: float, Flags: PUBLIC </summary>
        ParentRotation3,
        /// <summary> Type: uint, Flags: PUBLIC </summary>
        Faction,
        /// <summary> Type: uint, Flags: PUBLIC </summary>
        GameEntityLevel,
        /// <summary> Type: uint, Flags: PUBLIC, URGENT </summary>
        GameEntityInfo,
        /// <summary> Type: uint, Flags: PUBLIC, DYNAMIC, URGENT </summary>
        SpellVisualID,
        /// <summary> Type: uint, Flags: PUBLIC, DYNAMIC, URGENT </summary>
        StateSpellVisualID,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        StateAnimID,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        StateAnimKitID,
        /// <summary> Type: uint, Flags: DYNAMIC, URGENT </summary>
        StateWorldEffectID,
    }

    public enum DynamicEntityFields
    {
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        DynamicCaster = GameEntityFields.StateWorldEffectID + 1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicEntityType,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicSpellVisualID,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicSpellid,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        DynamicRadius,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DynamicCastTime,
    }

    public enum UnitFields
    {
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        Charm = DynamicEntityFields.DynamicCastTime + 1,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        Summon,
        /// <summary> Type: NetworkId, Flags: PRIVATE </summary>
        Critter,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        CharmedBy,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        SummonedBy,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        CreatedBy,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        DemonCreator,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        Target,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        BattlePetCompanionGuid,
        /// <summary> Type: NetworkId, Flags: PUBLIC, URGENT </summary>
        ChannelObject,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        ChannelSpell,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        ChannelSpellVisual,
        /// <summary> Type: Int - Bytes: 1 byte race, 1 byte class, 1 byte gender, Flags: PUBLIC </summary>
        Info,
        /// <summary> Type: Int - Powers, Flags: PUBLIC </summary>
        DisplayPower,
        /// <summary> Type: Int - Powers, Flags: PUBLIC </summary>
        OverrideDisplayPowerId,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        Health,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power1,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power2,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power3,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power4,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT_SELF_ONLY </summary>
        Power5,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxHealth,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower1,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower2,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower3,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower4,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        MaxPower5,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier4,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenFlatModifier5,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier4,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, UNIT_ALL </summary>
        PowerRegenInterruptedFlatModifier5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        Level,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        FactionTemplate,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId1,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId2,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId3,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId4,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        UnitVirtualItemSlotId5,
        /// <summary> Type: Int, Flags: PUBLIC, URGENT </summary>
        UnitFlags,
        /// <summary> Type: Int - AuraStateType, Flags: PUBLIC </summary>
        AuraState,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseAttackTimeMain,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseAttackTimeOffhand,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseAttackTimeRanged,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        BoundingRadius,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        CombatReach,
        /// <summary> Type: Uint, Flags: DYNAMIC, URGENT </summary>
        DisplayId,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        NativeDisplayId,
        /// <summary> Type: Uint, Flags: PUBLIC, URGENT </summary>
        MountDisplayId,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MinDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MaxDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MinOffhandDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        MaxOffhandDamage,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        PetNumber,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        PetNameTimestamp,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        PetExperience,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        PetNextLevelExp,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        UnitModCastSpeed,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        UnitModCastHaste,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModHaste,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModRangedHaste,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModHasteRegen,
        /// <summary> Type: Float, Flags: PUBLIC </summary>
        ModTimeRate,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        UnitCreatedBySpell,
        /// <summary> Type: Long - NpcFlags, Flags: PUBLIC, DYNAMIC </summary>
        UnitNpcFlags,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        Stat3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PosStat3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        NegStat3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER, SPECIAL_INFO </summary>
        Resistances6,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsPositive6,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        ResistanceBuffModsNegative6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseMana,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        BaseHealth,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        BaseFlags,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        AttackPower,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        AttackPowerModPos,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        AttackPowerModNeg,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        AttackPowerMultiplier,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        RangedAttackPower,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        RangedAttackPowerModPos,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        RangedAttackPowerModNeg,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        RangedAttackPowerMultiplier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        MinRangedDamage,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        MaxRangedDamage,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier1,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier2,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier3,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier4,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier5,
        /// <summary> Type: Uint, Flags: PRIVATE, OWNER </summary>
        PowerCostModifier6,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier1,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier2,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier3,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier4,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier5,
        /// <summary> Type: Float, Flags: PRIVATE, OWNER </summary>
        PowerCostMultiplier6,
    }

    public enum PlayerFields
    {
        /// <summary> Type: Long - PlayerFlags, Flags: PUBLIC </summary>
        PlayerFlags = UnitFields.PowerCostMultiplier6 + 1,
        /// <summary> Type: Int - ArenaTeam, Flags: PUBLIC </summary>
        ArenaTeam,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        DuelTeam,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ChosenTitle,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CurrentSpecId,
        /// <summary> Type: Long, Flags: PRIVATE </summary>
        Coinage,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        Xp,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        PlayerCharacterPoints,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        NextLevelXp,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        BlockPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        DodgePercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        DodgePercentageFromAttribute,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ParryPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ParryPercentageFromAttribute,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        CritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        RangedCritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        OffhandCritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        SpellCritPercentage,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ShieldBlock,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ShieldBlockCritPercentage,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        Mastery,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos1,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos2,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos3,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos4,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos5,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDonePos6,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg1,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg2,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg3,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg4,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg5,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        ModDamageDoneNeg6,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct1,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct2,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct3,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct4,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct5,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        ModDamageDonePct6,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        OverrideSpellPowerByApPct,
        /// <summary> Type: Float, Flags: PRIVATE </summary>
        OverrideApBySpellPowerPercent,
        /// <summary> Type: Int, Flags: PRIVATE </summary>
        ModTargetResistance,
        /// <summary> Type: Int, Flags: PRIVATE </summary>
        ModTargetPhysicalResistance,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating1,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating2,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating3,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating4,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating5,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating6,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating7,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating8,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating9,
        /// <summary> Type: Uint, Flags: PRIVATE </summary>
        CombatRating10,
    }

    public enum ItemFields
    {
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemOwner = PlayerFields.CombatRating10 + 1,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemContained,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemCreator,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ItemGiftcreator,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemStackCount,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemDuration,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges1,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges2,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges3,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemSpellCharges4,
        /// <summary> Type: Uint - ItemFieldFlags, Flags: PUBLIC </summary>
        ItemFlags,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentID12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentDuration12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemEnchantmentCharges12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemPropertySeed,
        /// <summary> Type: Int, Flags: PUBLIC </summary>
        ItemRandomPropertiesID,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemDurability,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemMaxdurability,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemCreatePlayedTime,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemModifiersMask,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ItemContext,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemArtifactXP,
        /// <summary> Type: Uint, Flags: OWNER </summary>
        ItemAppearanceModID,
    }

    public enum ContainerFields
    {
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot1 = ItemFields.ItemAppearanceModID + 1,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot2,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot3,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot4,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot5,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot6,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot7,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot8,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot9,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot10,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot11,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot12,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot13,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot14,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot15,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot16,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot17,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot18,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot19,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot20,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot21,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot22,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot23,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot24,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot25,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot26,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot27,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot28,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot29,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot30,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot31,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot32,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot33,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot34,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot35,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        ContainerFieldSlot36,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        ContainerFieldNumSlots,
    }

    public enum CorpseFields
    {
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        CorpseOwner = ContainerFields.ContainerFieldNumSlots + 1,
        /// <summary> Type: NetworkId, Flags: PUBLIC </summary>
        CorpseParty,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseDisplayID,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem1,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem2,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem3,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem4,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem5,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem6,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem7,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem8,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem9,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem10,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem11,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem12,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem13,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem14,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem15,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem16,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem17,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseItem18,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseInfo,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseExtraInfo,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseFlags,
        /// <summary> Type: Uint, Flags: DYNAMIC </summary>
        CorpseDynamicFlags,
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseFactiontemplate, 
        /// <summary> Type: Uint, Flags: PUBLIC </summary>
        CorpseCustomDisplayOption,
    }
}