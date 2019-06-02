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
        ImmunityEffect = 0,
        ImmunityState = 1,
        ImmunitySchool = 2,
        ImmunityDamage = 3,
        ImmunityDispel = 4,
        ImmunityMechanic = 5
    }

    public enum SpellCustomErrors
    {
        None = 0
    }
}