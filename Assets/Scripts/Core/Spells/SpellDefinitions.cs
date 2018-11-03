using System;
using UnityEngine;

namespace Core
{
    public enum SpellProcsPerMinuteModType
    {
        Haste = 1,
        Crit = 2,
        Class = 3,
        Spec = 4,
        Race = 5,
        Battleground = 6
    }

    public enum WeaponAttackType : byte
    {
        BaseAttack = 0,
        OffAttack = 1,
        RangedAttack = 2,
    }

    public enum SpellModType
    {
        Flat = 107,
        Percent = 108,
    }

    [Flags]
    public enum SpellChannelInterruptFlags
    {
        Interrupt = 0x08,
        Delay = 0x4000
    }

    [Flags]
    public enum ProcFlags
    {
        Killed = 0x00000001,
        Kill = 0x00000002,

        DoneMeleeAutoAttack = 0x00000004,
        TakenMeleeAutoAttack = 0x00000008,

        DoneSpellMeleeDmgClass = 0x00000010,
        TakenSpellMeleeDmgClass = 0x00000020,

        DoneRangedAutoAttack = 0x00000040,
        TakenRangedAutoAttack = 0x00000080,

        DoneSpellRangedDmgClass = 0x00000100,
        TakenSpellRangedDmgClass = 0x00000200,

        DoneSpellNoneDmgClassPos = 0x00000400,
        TakenSpellNoneDmgClassPos = 0x00000800,

        DoneSpellNoneDmgClassNeg = 0x00001000,
        TakenSpellNoneDmgClassNeg = 0x00002000,

        DoneSpellMagicDmgClassPos = 0x00004000,
        TakenSpellMagicDmgClassPos = 0x00008000,

        DoneSpellMagicDmgClassNeg = 0x00010000,
        TakenSpellMagicDmgClassNeg = 0x00020000,

        DonePeriodic = 0x00040000,
        TakenPeriodic = 0x00080000,

        TakenDamage = 0x00100000,
        DoneTrapActivation = 0x00200000,

        DoneMainhandAttack = 0x00400000,
        DoneOffhandAttack = 0x00800000,

        Death = 0x01000000,
        Jump = 0x02000000,

        EnterCombat = 0x08000000,
        EncounterStart = 0x10000000,


        AutoAttackMask = DoneMeleeAutoAttack | TakenMeleeAutoAttack
                                             | DoneRangedAutoAttack | TakenRangedAutoAttack,

        MeleeMask = DoneMeleeAutoAttack | TakenMeleeAutoAttack
                                        | DoneSpellMeleeDmgClass | TakenSpellMeleeDmgClass
                                        | DoneMainhandAttack | DoneOffhandAttack,

        RangedMask = DoneRangedAutoAttack | TakenRangedAutoAttack
                                          | DoneSpellRangedDmgClass | TakenSpellRangedDmgClass,

        SpellMask = DoneSpellMeleeDmgClass | TakenSpellMeleeDmgClass
                                           | DoneSpellRangedDmgClass | TakenSpellRangedDmgClass
                                           | DoneSpellNoneDmgClassPos | TakenSpellNoneDmgClassPos
                                           | DoneSpellNoneDmgClassNeg | TakenSpellNoneDmgClassNeg
                                           | DoneSpellMagicDmgClassPos | TakenSpellMagicDmgClassPos
                                           | DoneSpellMagicDmgClassNeg | TakenSpellMagicDmgClassNeg,

        SpellCastMask = SpellMask | DoneTrapActivation | RangedMask,

        PeriodicMask = DonePeriodic | TakenPeriodic,

        DoneHitMask = DoneMeleeAutoAttack | DoneRangedAutoAttack
                                          | DoneSpellMeleeDmgClass | DoneSpellRangedDmgClass
                                          | DoneSpellNoneDmgClassPos | DoneSpellNoneDmgClassNeg
                                          | DoneSpellMagicDmgClassPos | DoneSpellMagicDmgClassNeg
                                          | DonePeriodic | DoneMainhandAttack | DoneOffhandAttack,

        TakenHitMask = TakenMeleeAutoAttack | TakenRangedAutoAttack
                                            | TakenSpellMeleeDmgClass | TakenSpellRangedDmgClass
                                            | TakenSpellNoneDmgClassPos | TakenSpellNoneDmgClassNeg
                                            | TakenSpellMagicDmgClassPos | TakenSpellMagicDmgClassNeg
                                            | TakenPeriodic | TakenDamage,

        ReqSpellPhaseMask = SpellMask & DoneHitMask,

        MeleeBasedTriggerMask = DoneMeleeAutoAttack     | 
                                TakenMeleeAutoAttack    |
                                DoneSpellMeleeDmgClass  |
                                TakenSpellMeleeDmgClass |
                                DoneRangedAutoAttack    |
                                TakenRangedAutoAttack   |
                                DoneSpellRangedDmgClass |
                                TakenSpellRangedDmgClass
    }

    public enum SpellModOp
    {
        Damage = 0,
        Duration = 1,
        Threat = 2,
        Charges = 4,
        Range = 5,
        Radius = 6,
        CriticalChance = 7,
        AllEffects = 8,
        NotLoseCastingTime = 9,
        CastingTime = 10,
        Cooldown = 11,
        IgnoreArmor = 13,
        Cost = 14,
        CritDamageBonus = 15,
        ResistMissChance = 16,
        JumpTargets = 17,
        ChanceOfSuccess = 18,
        ActivationTime = 19,
        DamageMultiplier = 20,
        GlobalCooldown = 21,
        Dot = 22,
        BonusMultiplier = 24,
        ProcPerMinute = 26,
        ValueMultiplier = 27,
        ResistDispelChance = 28,
        StackAmount = 31,
        JumpDistance = 35,
    }

    [Flags]
    public enum SpellCastTargetFlags
    {
        Unit = 1 << 0,
        SourceLocation = 1 << 1,
        DestLocation = 1 << 2,
        UnitEnemy = 1 << 3,
        UnitAlly = 1 << 4,
        GameEntity = 1 << 5,

        [HideInInspector]
        UnitMask = Unit | UnitEnemy | UnitAlly,
    }

    public enum TargetSelections
    {
        Default,
        Channel,
        Nearby,
        Cone,
        Area
    }

    public enum TargetReferences
    {
        None,
        Caster,
        Target,
        Last,
        Source,
        Dest
    }

    public enum TargetEntities
    {
        None,
        Source,
        Dest,
        Unit,
        UnitAndDest,
        GameEntity,
    }

    public enum TargetChecks
    {
        Default,
        Entry,
        Enemy,
        Ally,
        Party,
    }

    public enum TargetDirections
    {
        None,
        Front,
        Back,
        Right,
        Left,
        FrontRight,
        BackRight,
        BackLeft,
        FrontLeft,
        Random,
        Entry
    }

    public enum ExplicitTargetTypes
    {
        None,
        Explicit,
        Caster,
    }

    [Flags]
    public enum SpellAttributes
    {
        Passive = 1 << 0,
        CastableWhileDead = 1 << 1,
        UnaffectedByInvulnerability = 1 << 3,
        CantCancel = 1 << 4,
        Channeled = 1 << 5,
        CantBeReflected = 1 << 6,
        DispelAurasOnImmunity = 1 << 7,
        UnaffectedBySchoolImmune = 1 << 8,
        CantTargetSelf = 1 << 9,
        CanTargetDead = 1 << 10,
        CantCrit = 1 << 11,
        TriggeredCanTriggerProc = 1 << 12,
        CanProcWithTriggered = 1 << 13,
        CantTriggerProc = 1 << 14,
        DisableProc = 1 << 15,
        BlockableSpell = 1 << 16,
        StackForDiffCasters = 1 << 17,
        OnlyTargetPlayers = 1 << 18,
        IgnoreHitResult = 1 << 19,
        DeathPersistent = 1 << 20,
    }

    [Flags]
    public enum SpellExtraAttributes
    {
        IgnoreResistances = 1 << 0,
        ProcOnlyOnCaster = 1 << 1,
        NotStealable = 1 << 2,
        CanCastWhileCasting = 1 << 3,
        FixedDamage = 1 << 4,
        DamageDoesntBreakAuras = 1 << 5,
        UsableWhileStunned = 1 << 6,
        SingleTargetSpell = 1 << 7,
        UsableWhileFeared = 1 << 8,
        UsableWhileConfused = 1 << 9,
        CanTargetInvisible = 1 << 10,
        OnlyVisibleToCaster = 1 << 11,
        CanTargetUntargetable = 1 << 12,
        DispelCharges = 1 << 13,
        SpecialDelayCalculation = 1 << 14,
        DisabledWhileActive = 1 << 15
    }

    [Flags]
    public enum SpellCustomAttributes
    {
        EnchantProc = 0x00000001,
        ConeBack = 0x00000002,
        ConeLine = 0x00000004,
        ShareDamage = 0x00000008,
        NoInitialThreat = 0x00000010,
        IsTalent = 0x00000020,
        DontBreakStealth = 0x00000040,
        DirectDamage = 0x00000100,
        Charge = 0x00000200,
        Pickpocket = 0x00000400,
        Negative = 0x00001000,
        IgnoreArmor = 0x00008000,
        ReqTargetFacingCaster = 0x00010000,
        ReqCasterBehindTarget = 0x00020000,
        AllowInflightTarget = 0x00040000,
        NeedsAmmoData = 0x00080000,
    }

    [Flags]
    public enum SpellCastFlags
    {
        None =  0,
        Pending = 1 << 0,
        HasTrajectory = 1 << 1,
        Projectile = 1 << 2,
        PowerLeftSelf = 1 << 3,
        AdjustMissile = 1 << 4,
        NoGcd = 1 << 5,
        VisualChain = 1 << 6,
        Immunity = 1 << 7,
    }

    public enum SpellCastSource
    {
        Player,
        Normal,
        Item,
        Passive,
        Pet,
        Aura,
        Spell,
    }

    [Flags]
    public enum SpellRangeFlag
    {
        Default = 0,
        Melee = 1,
        Ranged = 2,
    }

    public enum SpellState
    {
        None,
        Preparing,
        Casting,
        Finished,
        Idle,
        Delayed,
    }

    public enum SpellEffectHandleMode
    {
        Launch,
        LaunchTarget,
        Hit,
        HitTarget,
    }

    public enum SpellSchools
    {
        Normal,
        Holy,
        Fire,
        Nature,
        Frost,
        Shadow,
        Arcane,
    }

    public enum SpellCategory
    {
        Normal,
        Trap,
        MagicArmor,
        Potion,
        Seal,
        Aspect,
        Judgement,
        ControlRemoval
    }

    [Flags]
    public enum SpellSchoolMask
    {
        Normal = (1 << SpellSchools.Normal),
        Holy = (1 << SpellSchools.Holy),
        Fire = (1 << SpellSchools.Fire),
        Nature = (1 << SpellSchools.Nature),
        Frost = (1 << SpellSchools.Frost),
        Shadow = (1 << SpellSchools.Shadow),
        Arcane = (1 << SpellSchools.Arcane),

        SpellMask = (Fire | Nature | Frost | Shadow | Arcane),
        MagicMask = (Holy | SpellMask),
    }

    public enum PowerType
    {
        Mana,
        Rage,
        Focus,
        Energy,
        ComboPoints,
        Runes,
        RunicPower,
        SoulShards,
        LunarPower,
        HolyPower,
        AlternatePower,
        Maelstrom,
        Chi,
        Insanity,
        BurningEmbers,
        DemonicFury,
        ArcaneCharges,
        Fury,
        Pain,
        Health,
    }

    [Flags]
    public enum TriggerCastFlags
    {
        None = 0,                                   //! Not triggered
        IgnoreGcd = 1 << 0,                         //! Will ignore GCD
        IgnoreSpellAndCategoryCd = 1 << 1,          //! Will ignore Spell and Category cooldowns
        IgnorePowerAndReagentCost = 1 << 2,         //! Will ignore power and reagent cost
        IgnoreCastItem = 1 << 3,                    //! Will not take away cast item or update related achievement criteria
        IgnoreAuraScaling = 1 << 4,                 //! Will ignore aura scaling
        IgnoreCastInProgress = 1 << 5,              //! Will not check if a current cast is in progress
        IgnoreComboPoints = 1 << 6,                 //! Will ignore combo point requirement
        CastDirectly = 1 << 7,                      //! In Spell::prepare, will be cast directly without setting containers for executed spell
        IgnoreAuraInterruptFlags = 1 << 8,          //! Will ignore interruptible aura's at cast
        IgnoreSetFacing = 1 << 9,                   //! Will not adjust facing to target (if any)
        IgnoreShapeshift = 1 << 10,                 //! Will ignore shapeshift checks
        IgnoreCasterAurastate = 1 << 11,            //! Will ignore caster aura states including combat requirements and death state
        IgnoreCasterMountedOrOnVehicle = 1 << 12,   //! Will ignore mounted/on vehicle restrictions
        IgnoreCasterAuras = 1 << 13,                //! Will ignore caster aura restrictions or requirements
        DisallowProcEvents = 1 << 14,               //! Disallows proc events from triggered spell (default)
        DontReportCastError = 1 << 15,              //! Will return SPELL_FAILED_DONT_REPORT in CheckCast functions
        IgnoreEquippedItemRequirement = 1 << 16,    //! Will ignore equipped item requirements
        IgnoreTargetCheck = 1 << 17,                //! Will ignore most target checks (mostly DBC target checks)

        FullMask = 0x7fffffff,
    }

    public enum SpellCastResult
    {
        Success = 0,
        AffectingCombat = 1,
        AlreadyAtFullHealth = 2,
        AlreadyAtFullMana = 3,
        AlreadyAtFullPower = 4,
        AlreadyBeingTamed = 5,
        AlreadyHaveCharm = 6,
        AlreadyHaveSummon = 7,
        AlreadyHavePet = 8,
        AlreadyOpen = 9,
        AuraBounced = 10,
        AutotrackInterrupted = 11,
        BadImplicitTargets = 12,
        BadTargets = 13,
        PvpTargetWhileUnflagged = 14,
        CantBeCharmed = 15,
        CantBeDisenchanted = 16,
        CantBeDisenchantedSkill = 17,
        CantBeMilled = 18,
        CantBeProspected = 19,
        CantCastOnTapped = 20,
        CantDuelWhileInvisible = 21,
        CantDuelWhileStealthed = 22,
        CantStealth = 23,
        CantUntalent = 24,
        CasterAurastate = 25,
        CasterDead = 26,
        Charmed = 27,
        ChestInUse = 28,
        Confused = 29,
        DontReport = 30,
        EquippedItem = 31,
        EquippedItemClass = 32,
        EquippedItemClassMainhand = 33,
        EquippedItemClassOffhand = 34,
        Error = 35,
        Falling = 36,
        Fizzle = 37,
        Fleeing = 38,
        FoodLowlevel = 39,
        GarrisonNotOwned = 40,
        GarrisonOwned = 41,
        GarrisonMaxLevel = 42,
        GarrisonNotUpgradeable = 43,
        GarrisonFollowerOnMission = 44,
        GarrisonFollowerInBuilding = 45,
        GarrisonFollowerMaxLevel = 46,
        GarrisonFollowerMaxItemLevel = 47,
        GarrisonFollowerMaxQuality = 48,
        GarrisonFollowerNotMaxLevel = 49,
        GarrisonFollowerHasAbility = 50,
        GarrisonFollowerHasSingleMissionAbility = 51,
        GarrisonFollowerRequiresEpic = 52,
        GarrisonMissionNotInProgress = 53,
        GarrisonMissionComplete = 54,
        GarrisonNoMissionsAvailable = 55,
        Highlevel = 56,
        HungerSatiated = 57,
        Immune = 58,
        IncorrectArea = 59,
        Interrupted = 60,
        InterruptedCombat = 61,
        ItemAlreadyEnchanted = 62,
        ItemGone = 63,
        ItemNotFound = 64,
        ItemNotReady = 65,
        LevelRequirement = 66,
        LineOfSight = 67,
        LowLevel = 68,
        LowCastlevel = 69,
        MainhandEmpty = 70,
        Moving = 71,
        NeedAmmo = 72,
        NeedAmmoPouch = 73,
        NeedExoticAmmo = 74,
        NeedMoreItems = 75,
        Nopath = 76,
        NotBehind = 77,
        NotFishable = 78,
        NotFlying = 79,
        NotHere = 80,
        NotInfront = 81,
        NotInControl = 82,
        NotKnown = 83,
        NotMounted = 84,
        NotOnTaxi = 85,
        NotOnTransport = 86,
        NotReady = 87,
        NotShapeshift = 88,
        NotStanding = 89,
        NotTradeable = 90,
        NotTrading = 91,
        NotUnsheathed = 92,
        NotWhileGhost = 93,
        NotWhileLooting = 94,
        NoAmmo = 95,
        NoChargesRemain = 96,
        NoComboPoints = 97,
        NoDueling = 98,
        NoEndurance = 99,
        NoFish = 100,
        NoItemsWhileShapeshifted = 101,
        NoMountsAllowed = 102,
        NoPet = 103,
        NoPower = 104,
        NothingToDispel = 105,
        NothingToSteal = 106,
        OnlyAbovewater = 107,
        OnlyIndoors = 108,
        OnlyMounted = 109,
        OnlyOutdoors = 110,
        OnlyShapeshift = 111,
        OnlyStealthed = 112,
        OnlyUnderwater = 113,
        OutOfRange = 114,
        Pacified = 115,
        Possessed = 116,
        Reagents = 117,
        RequiresArea = 118,
        RequiresSpellFocus = 119,
        Rooted = 120,
        Silenced = 121,
        SpellInProgress = 122,
        SpellLearned = 123,
        SpellUnavailable = 124,
        Stunned = 125,
        TargetsDead = 126,
        TargetAffectingCombat = 127,
        TargetAurastate = 128,
        TargetDueling = 129,
        TargetEnemy = 130,
        TargetEnraged = 131,
        TargetFriendly = 132,
        TargetInCombat = 133,
        TargetInPetBattle = 134,
        TargetIsPlayer = 135,
        TargetIsPlayerControlled = 136,
        TargetNotDead = 137,
        TargetNotInParty = 138,
        TargetNotLooted = 139,
        TargetNotPlayer = 140,
        TargetNoPockets = 141,
        TargetNoWeapons = 142,
        TargetNoRangedWeapons = 143,
        TargetUnskinnable = 144,
        ThirstSatiated = 145,
        TooClose = 146,
        TooManyOfItem = 147,
        TotemCategory = 148,
        Totems = 149,
        TryAgain = 150,
        UnitNotBehind = 151,
        UnitNotInfront = 152,
        VisionObscured = 153,
        WrongPetFood = 154,
        NotWhileFatigued = 155,
        TargetNotInInstance = 156,
        NotWhileTrading = 157,
        TargetNotInRaid = 158,
        TargetFreeforall = 159,
        NoEdibleCorpses = 160,
        OnlyBattlegrounds = 161,
        TargetNotGhost = 162,
        TransformUnusable = 163,
        WrongWeather = 164,
        DamageImmune = 165,
        PreventedByMechanic = 166,
        PlayTime = 167,
        Reputation = 168,
        MinSkill = 169,
        NotInRatedBattleground = 170,
        NotOnShapeshift = 171,
        NotOnStealthed = 172,
        NotOnDamageImmune = 173,
        NotOnMounted = 174,
        TooShallow = 175,
        TargetNotInSanctuary = 176,
        TargetIsTrivial = 177,
        BmOrInvisgod = 178,
        GroundMountNotAllowed = 179,
        FloatingMountNotAllowed = 180,
        UnderwaterMountNotAllowed = 181,
        FlyingMountNotAllowed = 182,
        ApprenticeRidingRequirement = 183,
        JourneymanRidingRequirement = 184,
        ExpertRidingRequirement = 185,
        ArtisanRidingRequirement = 186,
        MasterRidingRequirement = 187,
        ColdRidingRequirement = 188,
        FlightMasterRidingRequirement = 189,
        CsRidingRequirement = 190,
        PandaRidingRequirement = 191,
        DraenorRidingRequirement = 192,
        MountNoFloatHere = 193,
        MountNoUnderwaterHere = 194,
        MountAboveWaterHere = 195,
        MountCollectedOnOtherChar = 196,
        NotIdle = 197,
        NotInactive = 198,
        PartialPlaytime = 199,
        NoPlaytime = 200,
        NotInBattleground = 201,
        NotInRaidInstance = 202,
        OnlyInArena = 203,
        TargetLockedToRaidInstance = 204,
        OnUseEnchant = 205,
        NotOnGround = 206,
        CustomError = 207,
        CantDoThatRightNow = 208,
        TooManySockets = 209,
        InvalidGlyph = 210,
        UniqueGlyph = 211,
        GlyphSocketLocked = 212,
        GlyphExclusiveCategory = 213,
        GlyphInvalidSpec = 214,
        GlyphNoSpec = 215,
        NoValidTargets = 216,
        ItemAtMaxCharges = 217,
        NotInBarbershop = 218,
        FishingTooLow = 219,
        ItemEnchantTradeWindow = 220,
        SummonPending = 221,
        MaxSockets = 222,
        PetCanRename = 223,
        TargetCannotBeResurrected = 224,
        TargetHasResurrectPending = 225,
        NoActions = 226,
        CurrencyWeightMismatch = 227,
        WeightNotEnough = 228,
        WeightTooMuch = 229,
        NoVacantSeat = 230,
        NoLiquid = 231,
        OnlyNotSwimming = 232,
        ByNotMoving = 233,
        InCombatResLimitReached = 234,
        NotInArena = 235,
        TargetNotGrounded = 236,
        ExceededWeeklyUsage = 237,
        NotInLfgDungeon = 238,
        BadTargetFilter = 239,
        NotEnoughTargets = 240,
        NoSpec = 241,
        CantAddBattlePet = 242,
        CantUpgradeBattlePet = 243,
        WrongBattlePetType = 244,
        NoDungeonEncounter = 245,
        NoTeleportFromDungeon = 246,
        MaxLevelTooLow = 247,
        CantReplaceItemBonus = 248,
        GrantPetLevelFail = 249,
        SkillLineNotKnown = 250,
        BlueprintKnown = 251,
        FollowerKnown = 252,
        CantOverrideEnchantVisual = 253,
        ItemNotAWeapon = 254,
        SameEnchantVisual = 255,
        ToyUseLimitReached = 256,
        ToyAlreadyKnown = 257,
        ShipmentsFull = 258,
        NoShipmentsForContainer = 259,
        NoBuildingForShipment = 260,
        NotEnoughShipmentsForContainer = 261,
        HasMission = 262,
        BuildingActivateNotReady = 263,
        NotSoulbound = 264,
        RidingVehicle = 265,
        VeteranTrialAboveSkillRankMax = 266,
        NotWhileMercenary = 267,
        SpecDisabled = 268,
        CantBeObliterated = 269,
        FollowerClassSpecCap = 270,
        TransportNotReady = 271,
        TransmogSetAlreadyKnown = 272,
        DisabledByAuraLabel = 273,
        DisabledByMaxUsableLevel = 274,
        SpellAlreadyKnown = 275,
        MustKnowSupercedingSpell = 276,
        YouCannotUseThatInPvpInstance = 277,
        NoArtifactEquipped = 278,
        WrongArtifactEquipped = 279,
        TargetIsUntargetableByAnyone = 280,
        SpellEffectFailed = 281,
        Unknown = 282,


        SpellCastOk = Success,
    }

    public enum DispelType
    {
        None,
        Magic,
        Curse,
        Disease,
        Poison,
        Stealth,
        Invisibility,
        Enrage,
    }

    public enum AuraStateType
    {
        None,
        Defense,
        Healthless20Percent,
        Berserking,
        Frozen,
        Judgement,
        HunterParry,
        WarriorVictoryRush,
        FairyFire,
        Healthless35Percent,
        Conflagrate,
        Swiftmend,
        DeadlyPoison,
        Enrage,
        Bleeding,
        HealthAbove75Percent,
    }

    public enum Mechanics
    {
        None,
        Charm,
        Disoriented,
        Disarm,
        Distract,
        Fear,
        Grip,
        Root,
        SlowAttack,
        Silence,
        Sleep,
        Snare,
        Stun,
        Freeze,
        Knockout,
        Bleed,
        Bandage,
        Polymorph,
        Banish,
        Shield,
        Shackle,
        Mount,
        Infected,
        Turn,
        Horror,
        Invulnerability,
        Interrupt,
        Daze,
        Discovery,
        ImmuneShield,
        Sapped,
        Enraged,
        Wounded,
    }

    public enum SpellDamageClass
    {
        None,
        Magic,
        Melee,
        Ranged,
    }

    [Flags]
    public enum SpellPreventionType
    {
        Silence = 1 << 0,
        Pacify = 1 << 1,
    }

    [Flags]
    public enum SpellInterruptFlags
    {
        Movement = 1 << 0,
        PushBack = 1 << 1,
        Interrupt = 1 << 2,
        AbortOnDmg = 1 << 3,
    }

    public enum SpellFamilyNames
    {
        Generic,
        Mage,
        Warrior,
        Warlock,
        Priest,
        Druid,
        Rogue,
        Hunter,
        Paladin,
        Shaman,
        Potion,
        Deathknight,
        Monk,
    }

    public enum SpellEffectType
    {
        None = 0,
        Instakill = 1,
        SchoolDamage = 2,
        Dummy = 3,
        PortalTeleport = 4,
        TeleportDirect = 5,
        ApplyAura = 6,
        EnvironmentalDamage = 7,
        PowerDrain = 8,
        HealthLeech = 9,
        Heal = 10,
        Bind = 11,
        Portal = 12,
        RitualBase = 13,
        IncreaseCurrencyCap = 14,
        RitualActivatePortal = 15,
        QuestComplete = 16,
        WeaponDamageNoschool = 17,
        Resurrect = 18,
        AddExtraAttacks = 19,
        Dodge = 20,
        Evade = 21,
        Parry = 22,
        Block = 23,
        CreateItem = 24,
        Weapon = 25,
        Defense = 26,
        PersistentAreaAura = 27,
        Summon = 28,
        Leap = 29,
        Energize = 30,
        WeaponPercentDamage = 31,
        TriggerMissile = 32,
        OpenLock = 33,
        SummonChangeItem = 34,
        ApplyAreaAuraParty = 35,
        LearnSpell = 36,
        SpellDefense = 37,
        Dispel = 38,
        Language = 39,
        DualWield = 40,
        Jump = 41,
        JumpDest = 42,
        TeleportUnitsFaceCaster = 43,
        SkillStep = 44,
        PlayMovie = 45,
        Spawn = 46,
        TradeSkill = 47,
        Stealth = 48,
        Detect = 49,
        TransDoor = 50,
        ForceCriticalHit = 51,
        SetMaxBattlePetCount = 52,
        EnchantItem = 53,
        EnchantItemTemporary = 54,
        TameCreature = 55,
        SummonPet = 56,
        LearnPetSpell = 57,
        WeaponDamage = 58,
        CreateRandomItem = 59,
        Proficiency = 60,
        SendEvent = 61,
        PowerBurn = 62,
        Threat = 63,
        TriggerSpell = 64,
        ApplyAreaAuraRaid = 65,
        CreateManaGem = 66,
        HealMaxHealth = 67,
        InterruptCast = 68,
        Distract = 69,
        Pull = 70,
        Pickpocket = 71,
        AddFarsight = 72,
        UntrainTalents = 73,
        ApplyGlyph = 74,
        HealMechanical = 75,
        SummonObjectWild = 76,
        ScriptEffect = 77,
        Attack = 78,
        Sanctuary = 79,
        AddComboPoints = 80,
        PushAbilityToActionBar = 81,
        BindSight = 82,
        Duel = 83,
        Stuck = 84,
        SummonPlayer = 85,
        ActivateObject = 86,
        GameobjectDamage = 87,
        GameobjectRepair = 88,
        GameobjectSetDestructionState = 89,
        KillCredit = 90,
        ThreatAll = 91,
        EnchantHeldItem = 92,
        ForceDeselect = 93,
        SelfResurrect = 94,
        Skinning = 95,
        Charge = 96,
        CastButton = 97,
        KnockBack = 98,
        Disenchant = 99,
        Inebriate = 100,
        FeedPet = 101,
        DismissPet = 102,
        Reputation = 103,
        SummonObjectSlot1 = 104,
        Survey = 105,
        ChangeRaidMarker = 106,
        ShowCorpseLoot = 107,
        DispelMechanic = 108,
        ResurrectPet = 109,
        DestroyAllTotems = 110,
        DurabilityDamage = 111,
        AttackMe = 114,
        DurabilityDamagePct = 115,
        SkinPlayerCorpse = 116,
        SpiritHeal = 117,
        Skill = 118,
        ApplyAreaAuraPet = 119,
        TeleportGraveyard = 120,
        NormalizedWeaponDmg = 121,
        SendTaxi = 123,
        PullTowards = 124,
        ModifyThreatPercent = 125,
        StealBeneficialBuff = 126,
        Prospecting = 127,
        ApplyAreaAuraFriend = 128,
        ApplyAreaAuraEnemy = 129,
        RedirectThreat = 130,
        PlaySound = 131,
        PlayMusic = 132,
        UnlearnSpecialization = 133,
        KillCredit2 = 134,
        CallPet = 135,
        HealPct = 136,
        EnergizePct = 137,
        LeapBack = 138,
        ClearQuest = 139,
        ForceCast = 140,
        ForceCastWithValue = 141,
        TriggerSpellWithValue = 142,
        ApplyAreaAuraOwner = 143,
        KnockBackDest = 144,
        PullTowardsDest = 145,
        ActivateRune = 146,
        QuestFail = 147,
        TriggerMissileSpellWithValue = 148,
        ChargeDest = 149,
        QuestStart = 150,
        TriggerSpell2 = 151,
        SummonRafFriend = 152,
        CreateTamedPet = 153,
        DiscoverTaxi = 154,
        TitanGrip = 155,
        EnchantItemPrismatic = 156,
        CreateItem2 = 157,
        Milling = 158,
        AllowRenamePet = 159,
        ForceCast2 = 160,
        TalentSpecCount = 161,
        TalentSpecSelect = 162,
        ObliterateItem = 163,
        RemoveAura = 164,
        DamageFromMaxHealthPct = 165,
        GiveCurrency = 166,
        UpdatePlayerPhase = 167,
        DestroyItem = 169,
        ResurrectWithAura = 172,
        RemoveTalent = 181,
        DespawnAreatrigger = 182,
        TeleportToDigsite = 191,
        UncageBattlepet = 192,
        StartPetBattle = 193,
        PlayScene = 198,
        HealBattlepetPct = 200,
        EnableBattlePets = 201,
        ChangeBattlepetQuality = 204,
        LaunchQuestChoice = 205,
    }

    public enum DiminishingLevels
    {
        Level1,
        Level2,
        Level3,
        Level4,
        Immune,
    }

    public enum DiminishingGroup
    {
        None,
        Root,
        Stun,
        Incapacitate,
        Disorient,
        Silence,
        AoeKnockback,
    }

    public enum SpellMissInfo
    {
        None,
        Miss,
        Resist,
        Dodge,
        Parry,
        Block,
        Evade,
        Immune,
        Deflect,
        Absorb,
        Reflect,
    }

    public enum DamageEffectType
    {
        DirectDamage = 0,
        SpellDirectDamage = 1,
        Dot = 2,
        Heal = 3,
        NoDamage = 4,
        SelfDamage = 5
    }

    public enum CurrentSpellTypes
    {
        MeleeSpell = 0,
        GenericSpell = 1,
        ChanneledSpell = 2,
        AutorepeatSpell = 3
    }

    public enum SpellImmunity
    {
        ImmunityEffect = 0,                     // enum SpellEffects
        ImmunityState = 1,                      // enum AuraType
        ImmunitySchool = 2,                     // enum SpellSchoolMask
        ImmunityDamage = 3,                     // enum SpellSchoolMask
        ImmunityDispel = 4,                     // enum DispelType
        ImmunityMechanic = 5,                   // enum Mechanics
    }

    public enum SpellCustomErrors
    {
        None = 0,
        CustomMsg = 1,  // Something bad happened, and we want to display a custom message!
        AlexBrokeQuest = 2,  // Alex broke your quest! Thank him later!
        NeedHelplessVillager = 3,  // This spell may only be used on Helpless Wintergarde Villagers that have not been rescued.
        NeedWarsongDisguise = 4,  // Requires that you be wearing the Warsong Orc Disguise.
        RequiresPlagueWagon = 5,  // You must be closer to a plague wagon in order to drop off your 7th Legion Siege Engineer.
        CantTargetFriendlyNonparty = 6,  // You cannot target friendly units outside your party.
        NeedChillNymph = 7,  // You must target a weakened chill nymph.
        MustBeInEnkilah = 8,  // The Imbued Scourge Shroud will only work when equipped in the Temple City of En'kilah.
        RequiresCorpseDust = 9,  // Requires Corpse Dust
        CantSummonGargoyle = 10,  // You cannot summon another gargoyle yet.
        NeedCorpseDustIfNoTarget = 11,  // Requires Corpse Dust if the target is not dead and humanoid.
        MustBeAtShatterhorn = 12,  // Can only be placed near Shatterhorn
        MustTargetProtoDrakeEgg = 13,  // You must first select a Proto-Drake Egg.
        MustBeCloseToTree = 14,  // You must be close to a marked tree.
        MustTargetTurkey = 15,  // You must target a Fjord Turkey.
        MustTargetHawk = 16,  // You must target a Fjord Hawk.
        TooFarFromBouy = 17,  // You are too far from the bouy.
        MustBeCloseToOilSlick = 18,  // Must be used near an oil slick.
        MustBeCloseToBouy = 19,  // You must be closer to the buoy!
        WyrmrestVanquisher = 20,  // You may only call for the aid of a Wyrmrest Vanquisher in Wyrmrest Temple, The Dragon Wastes, Galakrond's Rest or The Wicked Coil.
        MustTargetIceHeartJormungar = 21,  // That can only be used on a Ice Heart Jormungar Spawn.
        MustBeCloseToSinkhole = 22,  // You must be closer to a sinkhole to use your map.
        RequiresHaroldLane = 23,  // You may only call down a stampede on Harold Lane.
        RequiresGammothMagnataur = 24,  // You may only use the Pouch of Crushed Bloodspore on Gammothra or other magnataur in the Bloodspore Plains and Gammoth.
        MustBeInResurrectionChamber = 25,  // Requires the magmawyrm resurrection chamber in the back of the Maw of Neltharion.
        CantCallWintergardeHere = 26,  // You may only call down a Wintergarde Gryphon in Wintergarde Keep or the Carrion Fields.
        MustTargetWilhelm = 27,  // What are you doing? Only aim that thing at Wilhelm!
        NotEnoughHealth = 28,  // Not enough health!
        NoNearbyCorpses = 29,  // There are no nearby corpses to use.
        TooManyGhouls = 30,  // You've created enough ghouls. Return to Gothik the Harvester at Death's Breach.
        GOFurtherFromSunderedShard = 31,  // Your companion does not want to come here.  Go further from the Sundered Shard.
        MustBeInCatForm = 32,  // Must be in Cat Form
        MustBeDeathKnight = 33,  // Only Death Knights may enter Ebon Hold.
        MustBeInBearForm = 34,  // Must be in Bear Form
        MustBeNearHelplessVillager = 35,  // You must be within range of a Helpless Wintergarde Villager.
        CantTargetElementalMechanical = 36,  // You cannot target an elemental or mechanical corpse.
        MustHaveUsedDalaranCrystal = 37,  // This teleport crystal cannot be used until the teleport crystal in Dalaran has been used at least once.
        YouAlreadyHoldSomething = 38,  // You are already holding something in your hand. You must throw the creature in your hand before picking up another.
        YouDontHoldAnything = 39,  // You don't have anything to throw! Find a Vargul and use Gymer Grab to pick one up!
        MustBeCloseToValduran = 40,  // Bouldercrag's War Horn can only be used within 10 yards of Valduran the Stormborn.
        NoPassenger = 41,  // You are not carrying a passenger. There is nobody to drop off.
        CantBuildMoreVehicles = 42,  // You cannot build any more siege vehicles.
        AlreadyCarryingCrusader = 43,  // You are already carrying a captured Argent Crusader. You must return to the Argent Vanguard infirmary and drop off your passenger before you may pick up another.
        CantDoWhileRooted = 44,  // You can't do that while rooted.
        RequiresNearbyTarget = 45,  // Requires a nearby target.
        NothingToDiscover = 46,  // Nothing left to discover.
        NotEnoughTargets = 47,  // No targets close enough to bluff.
        ConstructTooFar = 48,  // Your Iron Rune Construct is out of range.
        RequiresGrandMasterEngineer = 49,  // Requires Engineering (350)
        CantUseThatMount = 50,  // You can't use that mount.
        NooneToEject = 51,  // There is nobody to eject!
        TargetMustBeBound = 52,  // The target must be bound to you.
        TargetMustBeUndead = 53,  // Target must be undead.
        TargetTooFar = 54,  // You have no target or your target is too far away.
        MissingDarkMatter = 55,  // Missing Reagents: Dark Matter
        CantUseThatItem = 56,  // You can't use that item
        CantDoWhileCycyloned = 57,  // You can't do that while Cycloned
        TargetHasScroll = 58,  // Target is already affected by a similar effect
        PoisonTooStrong = 59,  // That anti-venom is not strong enough to dispel that poison
        MustHaveLanceEquipped = 60,  // You must have a lance equipped.
        MustBeCloseToMaiden = 61,  // You must be near the Maiden of Winter's Breath Lake.
        LearnedEverything = 62,  // You have learned everything from that book
        PetIsDead = 63,  // Your pet is dead
        NoValidTargets = 64,  // There are no valid targets within range.
        GMOnly = 65,  // Only GMs may use that. Your account has been reported for investigation.
        RequiresLevel58 = 66,  // You must reach level 58 to use this portal.
        AtHonorCap = 67,  // You already have the maximum amount of honor.
        HaveHotRod = 68,  // You already have a Hot Rod.
        PartygoerMoreBubbly = 69,  // This partygoer wants some more bubbly.
        PartygoerNeedBucket = 70,  // This partygoer needs a bucket!
        PartygoerWantToDance = 71,  // This partygoer wants to dance with you.
        PartygoerWantFireworks = 72,  // This partygoer wants to see some fireworks.
        PartygoerWantAppetizer = 73,  // This partygoer wants some more hors d'oeuvres.
        GoblinBatteryDepleted = 74,  // The Goblin All-In-1-Der Belt's battery is depleted.
        MustHaveDemonicCircle = 75,  // You must have a demonic circle active.
        AtMaxRage = 76,  // You already have maximum rage
        Requires350Engineering = 77,  // Requires Engineering (350)
        SoulBelongsToLichKing = 78,  // Your soul belongs to the Lich King
        AttendantHasPony = 79,  // Your attendant already has an Argent Pony
        GoblinStartingMission = 80,  // First, Overload the Defective Generator, Activate the Leaky Stove, and Drop a Cigar on the Flammable Bed.
        GasbotAlreadySent = 81,  // You've already sent in the Gasbot and destroyed headquarters!
        GoblinIsPartiedOut = 82,  // This goblin is all partied out!
        MustHaveFireTotem = 83,  // You must have a Magma, Flametongue, or Fire Elemental Totem active.
        CantTargetVampires = 84,  // You may not bite other vampires.
        PetAlreadyAtYourLevel = 85,  // Your pet is already at your level.
        MissingItemRequiremens = 86,  // You do not meet the level requirements for this item.
        TooManyAbominations = 87,  // There are too many Mutated Abominations.
        AllPotionsUsed = 88,  // The potions have all been depleted by Professor Putricide.
        DefeatedEnoughAlready = 89,  // You have already defeated enough of them.
        RequiresLevel65 = 90,  // Requires level 65
        DestroyedKtcOilPlatform = 91,  // You have already destroyed the KTC Oil Platform.
        LaunchedEnoughCages = 92,  // You have already launched enough cages.
        RequiresBoosterRockets = 93,  // Requires Single-Stage Booster Rockets. Return to Hobart Grapplehammer to get more.
        EnoughWildCluckers = 94,  // You have already captured enough wild cluckers.
        RequiresControlFireworks = 95,  // Requires Remote Control Fireworks. Return to Hobart Grapplehammer to get more.
        MaxNumberOfRecruits = 96,  // You already have the max number of recruits.
        MaxNumberOfVolunteers = 97,  // You already have the max number of volunteers.
        FrostmourneRenderedResurrect = 98,  // Frostmourne has rendered you unable to resurrect.
        CantMountWithShapeshift = 99,  // You can't mount while affected by that shapeshift.
        FawnsAlreadyFollowing = 100, // Three fawns are already following you!
        AlreadyHaveRiverBoat = 101, // You already have a River Boat.
        NoActiveEnchantment = 102, // You have no active enchantment to unleash.
        EnoughHighbourneSouls = 103, // You have bound enough Highborne souls. Return to Arcanist Valdurian.
        Atleast40YdFromOilDrilling = 104, // You must be at least 40 yards away from all other Oil Drilling Rigs.
        AboveEnslavedPearlMiner = 106, // You must be above the Enslaved Pearl Miner.
        MustTargetCorpseSpecial1 = 107, // You must target the corpse of a Seabrush Terrapin, Scourgut Remora, or Spinescale Hammerhead.
        SlaghammerAlreadyPrisoner = 108, // Ambassador Slaghammer is already your prisoner.
        RequireAttunedLocation1 = 109, // Requires a location that is attuned with the Naz'jar Battlemaiden.
        NeedToFreeDrakeFirst = 110, // Free the Drake from the net first!
        DragonmawAlliesAlreadyFollow = 111, // You already have three Dragonmaw allies following you.
        RequireOpposableThumbs = 112, // Requires Opposable Thumbs.
        NotEnoughHealth2 = 113, // Not enough health
        EnoughForsakenTroopers = 114, // You already have enough Forsaken Troopers.
        CannotJumpToBoulder = 115, // You cannot jump to another boulder yet.
        SkillTooHigh = 116, // Skill too high.
        Already6SurvivorsRescued = 117, // You have already rescued 6 Survivors.
        MustFaceShipsFromBalloon = 118, // You need to be facing the ships from the rescue balloon.
        CannotSuperviseMoreCultists = 119, // You cannot supervise more than 5 Arrested Cultists at a time.
        RequiresLevel85 = 120, // You must reach level 85 to use this portal.
        MustBeBelow35Health = 121, // Your target must be below 35% health.
        MustSelectSpecialization = 122, // You must select a specialization first.
        TooWiseAndPowerful = 123, // You are too wise and powerful to gain any benefit from that item.
        TooCloseArgentLightwell = 124, // You are within 10 yards of another Argent Lightwell.
        NotWhileShapeshifted = 125, // You can't do that while shapeshifted.
        ManaGemInBank = 126, // You already have a Mana Gem in your bank.
        FlameShockNotActive = 127, // You must have at least one Flame Shock active.
        CantTransform = 128, // You cannot transform right now
        PetMustBeAttacking = 129, // Your pet must be attacking a target.
        GnomishEngineering = 130, // Requires Gnomish Engineering
        GoblinEngineering = 131, // Requires Goblin Engineering
        NoTarget = 132, // You have no target.
        PetOutOfRange = 133, // Your Pet is out of range of the target.
        HoldingFlag = 134, // You can't do that while holding the flag.
        TargetHoldingFlag = 135, // You can't do that to targets holding the flag.
        PortalNotOpen = 136, // The portal is not yet open.  Continue helping the druids at the Sanctuary of Malorne.
        AggraAirTotem = 137, // You need to be closer to Aggra's Air Totem, in the west.
        AggraWaterTotem = 138, // You need to be closer to Aggra's Water Totem, in the north.
        AggraEarthTotem = 139, // You need to be closer to Aggra's Earth Totem, in the east.
        AggraFireTotem = 140, // You need to be closer to Aggra's Fire Totem, near Thrall.
        FacingWrongWay = 141, // You are facing the wrong way.
        TooCloseToMakeshiftDynamite = 142, // You are within 10 yards of another Makeshift Dynamite.
        NotNearSapphireSunkenShip = 143, // You must be near the sunken ship at Sapphire's End in the Jade Forest.
        DemonsHealthFull = 144, // That demon's health is already full.
        OnyxSerpentNotOverhead = 145, // Wait until the Onyx Serpent is directly overhead.
        ObjectiveAlreadyComplete = 146, // Your objective is already complete.
        PushSadPandaTowardsTown = 147, // You can only push Sad Panda towards Sad Panda Town!
        TargetHasStartdust2 = 148, // Target is already affected by Stardust No. 2.
        ElementiumGemClusters = 149, // You cannot deconstruct Elementium Gem Clusters while collecting them!
        YouDontHaveEnoughHealth = 150, // You don't have enough health.
        YouCannotUseTheGatewayYet = 151, // You cannot use the gateway yet.
        ChooseSpecForAscendance = 152, // You must choose a specialization to use Ascendance.
        InsufficientBloodCharges = 153, // You have insufficient Blood Charges.
        NoFullyDepletedRunes = 154, // No fully depleted runes.
        NoMoreCharges = 155, // No more charges.
        StatueIsOutOfRangeOfTarget = 156, // Statue is out of range of the target.
        YouDontHaveAStatueSummoned = 157, // You don't have a statue summoned.
        YouHaveNoSpiritActive = 158, // You have no spirit active.
        BothDisesasesMustBeOnTarget = 159, // Both Frost Fever and Blood Plague must be present on the target.
        CantDoThatWithOrbOfPower = 160, // You can't do that while holding an Orb of Power.
        CantDoThatWhileJumpingOrFalling = 161, // You can't do that while jumping or falling.
        MustBeTransformedByPolyformicAcid = 162, // You must be transformed by Polyformic Acid.
        NotEnoughAcidToStoreTransformation = 163, // There isn't enough acid left to store this transformation.
        MustHaveFlightMastersLicense = 164, // You must obtain a Flight Master's License before using this spell.
        AlreadySampledSapFromFeeder = 165, // You have already sampled sap from this Feeder.
        MustBeNewrMantidFeeder = 166, // Requires you to be near a Mantid Feeder in the Heart of Fear.
        TargetMustBeInDirectlyFront = 167, // Target must be directly in front of you.
        CantDoThatWhileMythicKeystoneIsActive = 168, // You can't do that while a Mythic Keystone is active.
        WrongClassForMount = 169, // You are not the correct class for that mount.
        NothingLeftToDiscover = 170, // Nothing left to discover.
        NoExplosivesAvailable = 171, // There are no explosives available.
        YouMustBeFlaggedForPVP = 172, // You must be flagged for PvP.
        RequiresBattleRations = 173, // Requires Battle Rations or Meaty Haunch
        RequiresBrittleRoot = 174, // Requires Brittle Root
        RequiresLaborersTool = 175, // Requires Laborer's Tool
        RequiresUnexplodedCannonball = 176, // Requires Unexploded Cannonball
        RequiresMisplacedKeg = 177, // Requires Misplaced Keg
        RequiresLiquidFire = 178, // Requires Liquid Fire, Jungle Hops, or Spirit-kissed Water
        RequiresKrasariIron = 179, // Requires Krasari Iron
        RequiresSpiritKissedWater = 180, // Requires Spirit-Kissed Water
        RequiresSnakeOil = 181, // Requires Snake Oil
        ScenarioIsInProgress = 182, // You can't do that while a Scenario is in progress.
        RequiresDarkmoonFaireOpen = 183, // Requires the Darkmoon Faire to be open.
        AlreadyAtValorCap = 184, // Already at Valor cap
        AlreadyCommendedByThisFaction = 185, // Already commended by this faction
        OutOfCoins = 186, // Out of coins! Pickpocket humanoids to get more.
        OnlyOneElementalSpirit = 187, // Only one elemental spirit on a target at a time.
        DontKnowHowToTameDirehorns = 188, // You do not know how to tame Direhorns.
        MustBeNearBloodiedCourtGate = 189, // You must be near the Bloodied Court gate.
        YouAreNotElectrified = 190, // You are not Electrified.
        ThereIsNothingToBeFetched = 191, // There is nothing to be fetched.
        RequiresTheThunderForge = 192, // Requires The Thunder Forge.
        CannotUseTheDiceAgainYet = 193, // You cannot use the dice again yet.
        AlreadyMemberOfBrawlersGuild = 194, // You are already a member of the Brawler's Guild.
        CantChangeSpecInCelestialChallenge = 195, // You may not change talent specializations during a celestial challenge.
        SpecDoesMatchChallenge = 196, // Your talent specialization does not match the selected challenge.
        YouDontHaveEnoughCurrency = 197, // You don't have enough currency to do that.
        TargetCannotBenefitFromSpell = 198, // Target cannot benefit from that spell
        YouCanOnlyHaveOneHealingRain = 199, // You can only have one Healing Rain active at a time.
        TheDoorIsLocked = 200, // The door is locked.
        YouNeedToSelectWaitingCustomer = 201, // You need to select a customer who is waiting in line first.
        CantChangeSpecDuringTrial = 202, // You may not change specialization while a trial is in progress.
        CustomerNeedToGetInLine = 203, // You must wait for customers to get in line before you can select them to be seated.
        MustBeCloserToGazloweObjective = 204, // Must be closer to one of Gazlowe's objectives to deploy!
        MustBeCloserToThaelinObjective = 205, // Must be closer to one of Thaelin's objectives to deploy!
        YourPackOfVolenIsFull = 206, // Your pack of volen is already full!
        Requires600MiningOrBlacksmithing = 207, // Requires 600 Mining or Blacksmithing
        ArkoniteProtectorNotInRange = 208, // The Arkonite Protector is not in range.
        TargetCannotHaveBothBeacons = 209, // You are unable to have both Beacon of Light and Beacon of Faith on the same target.
        CanOnlyUseOnAFKPlayer = 210, // Can only be used on AFK players.
        NoLootableCorpsesInRange = 211, // No lootable corpse in range
        ChimaeronTooCalmToTame = 212, // Chimaeron is too calm to tame right now.
        CanOnlyCarryOneTypeOfMunitions = 213, // You may only carry one type of Blackrock Munitions.
        OutOfBlackrockMunitions = 214, // You have run out of Blackrock Munitions.
        CarryingMaxAmountOfMunitions = 215, // You are carrying the maximum amount of Blackrock Munitions.
        TargetIsTooFarAway = 216, // Target is too far away.
        CannotUseDuringBossEncounter = 217, // Cannot use during a boss encounter.
        MustHaveMeleeWeaponInBothHands = 218, // Must have a Melee Weapon equipped in both hands
        YourWeaponHasOverheated = 219, // Your weapon has overheated.
        MustBePartyLeaderToQueue = 220, // You must be a party leader to queue your group.
        NotEnoughFuel = 221, // Not enough fuel
        YouAreAlreadyDisguised = 222, // You are already disguised!
        YouNeedToBeInShredder = 223, // You need to be in a Shredder to chop this up!
        FoodCannotEatFood = 224, // Food cannot eat food
        MysteriousForcePreventsOpeningChest = 225, // A mysterious force prevents you from opening the chest.
        CantDoThatWhileHoldingEmpoweredOre = 226, // You can't do that while holding Empowered Ore.
        NotEnoughAmmunition = 227, // Not enough Ammunition!
        YouNeedBeatfaceTheGladiator = 228, // You need Beatface the Sparring Arena gladiator to break this!
        YouCanOnlyHaveOneWaygate = 229, // You can only have one waygate open. Disable an activated waygate first.
        YouCanOnlyHaveTwoWaygates = 230, // You can only have two waygates open. Disable an activated waygate first.
        YouCanOnlyHaveThreeWaygates = 231, // You can only have three waygates open. Disable an activated waygate first.
        RequiresMageTower = 232, // Requires Mage Tower
        RequiresSpiritLodge = 233, // Requires Spirit Lodge
        FrostWyrmAlreadyActive = 234, // A Frost Wyrm is already active.
        NotEnoughRunicPower = 235, // Not enough Runic Power
        YouAreThePartyLeader = 236, // You are the Party Leader.
        YulonIsAlreadyActive = 237, // Yu'lon is already active.
        AStampedeIsAlreadyActive = 238, // A Stampede is already active.
        YouAreAlreadyWellFed = 239, // You are already Well Fed.
        CantDoThatUnderSuppressiveFire = 240, // You cannot do that while under Suppressive Fire.
        YouAlreadyHaveMurlocSlop = 241, // You already have a piece of Murloc Slop.
        YouDontHaveArtifactFragments = 242, // You don't have any Artifact Fragments.
        YouArentInAParty = 243, // You aren't in a Party.
        Requires20Ammunition = 244, // Requires 30 Ammunition!
        Requires30Ammunition = 245, // Requires 20 Ammunition!
        YouAlreadyHaveMaxOutcastFollowers = 246, // You already have the maximum amount of Outcasts following you.
        NotInWorldPVPZone = 247, // Not in World PvP zone.
        AlreadyAtResourceCap = 248, // Already at Resource cap
        ApexisSentinelRequiresEnergy = 249, // This Apexis Sentinel requires energy from a nearby Apexis Pylon to be powered up.
        YouMustHave3OrFewerPlayer = 250, // You must have 3 or fewer players.
        YouAlreadyReadTreasureMap = 251, // You have already read that treasure map.
        MayOnlyUseWhileGarrisonUnderAttack = 252, // You may only use this item while your garrison is under attack.
        RequiresActiveMushrooms = 253, // This spell requires active mushrooms for you to detonate.
        RequiresFasterTimeWithRacer = 254, // Requires a faster time with the basic racer
        RequiresInfernoShotAmmo = 255, // Requires Inferno Shot Ammo!
        YouCannotDoThatRightNow = 256, // You cannot do that right now.
        ATrapIsAlreadyPlacedThere = 257, // A trap is already placed there.
        YouAreAlreadyOnThatQuest = 258, // You are already on that quest.
        RequiresFelforgedCudgel = 259, // Requires a Felforged Cudgel!
        CantTakeWhileBeingDamaged = 260, // Can't take while being damaged!
        YouAreBoundToDraenor = 261, // You are bound to Draenor by Archimonde's magic.
        AlreayHaveMaxNumberOfShips = 262, // You already have the maximum number of ships your shipyard can support.
        MustBeAtShipyard = 263, // You must be at your shipyard.
        RequiresLevel3MageTower = 264, // Requires a level 3 Mage Tower.
        RequiresLevel3SpiritLodge = 265, // Requires a level 3 Spirit Lodge.
        YouDoNotLikeFelEggsAndHam = 266, // You do not like Fel Eggs and Ham.
        AlreadyEnteredInThisAgreement = 267, // You have already entered in to this trade agreement.
        CannotStealThatWhileGuardsAreOnDuty = 268, // You cannot steal that while guards are on duty.
        YouAlreadyUsedVantusRune = 269, // You have already used a Vantus Rune this week.
        ThatItemCannotBeObliterated = 270, // That item cannot be obliterated.
        NoSkinnableCorpseInRange = 271, // No skinnable corpse in range
        MustBeMercenaryToUseTrinket = 272, // You must be a Mercenary to use this trinket.
        YouMustBeInCombat = 273, // You must be in combat.
        NoEnemiesNearTarget = 274, // No enemies near target.
        RequiresLeyspineMissile = 275, // Requires a Leyspine Missile
        RequiresBothCurrentsConnected = 276, // Requires both currents connected.
        CantDoThatInDemonForm = 277, // Can't do that while in demon form (yet)
        YouDontKnowHowToTameMechs = 278, // You do not know how to tame or obtain lore about Mechs.
        CannotCharmAnyMoreWithered = 279, // You cannot charm any more withered.
        RequiresActiveHealingRain = 280, // Requires an active Healing Rain.
        AlreadyCollectedAppearances = 281, // You've already collected these appearances
        CannotResurrectSurrenderedToMadness = 282, // Cannot resurrect someone who has surrendered to madness
        YouMustBeInCatForm = 283, // You must be in Cat Form.
        YouCannotReleaseSpiritYet = 284, // You cannot Release Spirit yet.
        NoFishingNodesNearby = 285, // No fishing nodes nearby.
        YouAreNotInCorrectSpec = 286, // You are not the correct specialization.
        UlthaleshHasNoPowerWithoutSouls = 287, // Ulthalesh has no power without souls.
        CannotCastThatWithVoodooTotem = 288, // You cannot cast that while talented into Voodoo Totem.
        AlreadyCollectedThisAppearance = 289, // You've already collected this appearance.
        YourPetMaximumIsAlreadyHigh = 290, // Your total pet maximum is already this high.
        YouDontHaveEnoughWithered = 291, // You do not have enough withered to do that.
        RequiresNearbySoulFragment = 292, // Requires a nearby Soul Fragment.
        RequiresAtLeast10Withered = 293, // Requires at least 10 living withered
        RequiresAtLeast14Withered = 294, // Requires at least 14 living withered
        RequiresAtLeast18Withered = 295, // Requires at least 18 living withered
        Requires2WitheredManaRagers = 296, // Requires 2 Withered Mana-Ragers
        Requires1WitheredBerserke = 297, // Requires 1 Withered Berserker
        Requires2WitheredBerserker = 298, // Requires 2 Withered Berserkers
        TargetHealthIsTooLow = 299, // Target's health is too low
        CannotShapeshiftWhileRidingStormtalon = 300, // You cannot shapeshift while riding Stormtalon
        CannotChangeSpecInCombatTraining = 301, // You can not change specializations while in Combat Training.
        UnknownPhenomenonPreventsLeylineConnection = 302, // Unknown phenomenon is preventing a connection to the Leyline.
        TheNightmareObscuresYourVision = 303, // The Nightmare obscures your vision.
        YouAreInWrongClassSpec = 304, // You are in the wrong class specialization.
        ThereAreNoValidCorpsesNearby = 305, // There are no valid corpses nearby.
        CantCastThatRightNow = 306, // Can't cast that right now.
        NotEnoughAncientMan = 307, // Not enough Ancient Mana.
        RequiresSongScroll = 308, // Requires a Song Scroll to function.
        MustHaveArtifactEquipped = 309, // You must have an artifact weapon equipped.
        RequiresCatForm = 310, // Requires Cat Form.
        RequiresBearForm = 311, // Requires Bear Form.
        RequiresConjuredFood = 312, // Requires either a Conjured Mana Pudding or Conjured Mana Fritter.
        RequiresArtifactWeapon = 313, // Requires an artifact weapon.
        YouCantCastThatHere = 314, // You can't cast that here
        CantDoThatOnClassTrial = 315, // You cannot do that while on a Class Trial.
        RitualOfDoomOncePerDay = 316, // You can only benefit from the Ritual of Doom once per day.
        CannotRitualOfDoomWhileSummoningSiters = 317, // You cannot perform the Ritual of Doom while attempting to summon the sisters.
        LearnedAllThatYouCanAboutYourArtifact = 318, // You have learned all that you can about your artifact.
        CantCallPetWithLoneWolf = 319, // You cannot use Call Pet while Lone Wolf is active.
        YouMustBeInAnInnToStrumThatGuitar = 321, // You must be in an inn to strum that guitar.
        YouCannotReachTheLatch = 322, // You cannot reach the latch.
        YouMustBeWieldingTheUnderlightAngler = 323, // You must be wielding the Underlight Angler.
    }
}