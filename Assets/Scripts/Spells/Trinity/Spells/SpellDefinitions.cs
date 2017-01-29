using System;

[Flags]
public enum SpellCastTargetFlags
{
    None = 0,
    Unit = 1 << 0,              // Id
    UnitDead = 1 << 1,          // Not sent, used to validate target (if dead creature)
    UnitEnemy = 1 << 2,         // Not sent, used to validate target (if enemy)
    UnitAlly = 1 << 3,          // Not sent, used to validate target (if ally)

    CorpseEnemy = 1 << 4,       // Id
    CorpseAlly = 1 << 5,        // Id

    SourceLocation = 1 << 6,    // Id, Vector3
    DestLocation = 1 << 7,      // Id, Vector3
    Gameobject = 1 << 8,        // Id

    UnitMask = Unit | UnitEnemy | UnitAlly | UnitDead,
    CorpseMask = CorpseAlly | CorpseEnemy,
};

public enum TargetSelections
{
    Default,
    Channel,
    Nearby,
    Cone,
    Area,
};

public enum TargetReferences
{
    None,
    Caster,
    Target,
    Last,
    Source,
    Dest,
};

public enum TargetObjects
{
    None,
    Source,
    Dest,
    Unit,
    UnitAndDest,
    Item,
    Corpse,
    CorpseEnemy,
    CorpseAlly
};

public enum TargetChecks
{
    Default,
    Entry,
    Enemy,
    Ally,
};

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
    Entry,
};

public enum SpellEffectImplicitTargetTypes
{
    None,
    Explicit,
    Caster,
};

public enum SpellSpecificType
{
    Normal,
    Seal,
    Aura3,
    Sting,
    Curse,
    Aspect,
    Tracker,
    WarlockArmor,
    MageArmor,
    ElementalShield,
    MagePolymorph,
    Judgement,
    WarlockCorruption,
    Food,
    Drink,
    FoodAndDrink,
    Presence,
    Charm,
    Scroll,
    MageArcaneBrillance,
    WarriorEnrage,
    PriestDivineSpirit,
    Hand,
    Phase,
    Bane,
};

[Flags]
public enum SpellAttributes : ulong
{
    None = 0,
    ConeBack = 1 << 0,
    ConeLine = 1 << 1,
    Charge = 1 << 2,
    NegativeEff0 = 1 << 3,
    NegativeEff1 = 1 << 4,
    NegativeEff2 = 1 << 5,
    IgnoreArmor = 1 << 6,
    ReqTargetFacingCaster = 1 << 7,
    ReqCasterBehindTarget = 1 << 8,
    CantCrit = 1 << 9,
    TriggeredCanTriggerProc = 1 << 10,
    StackForDiffCasters = 1 << 11,
    OnlyTargetPlayers = 1 << 12,
    IgnoreHitResult = 1 << 13,
    IgnoreResistances = 1 << 14,
    ProcOnlyOnCaster = 1 << 15,
    NotStealable = 1 << 16,
    CanCastWhileCasting = 1 << 17,
    FixedDamage = 1 << 18,
    TriggerActivate = 1 << 19,
    DamageDoesntBreakAuras = 1 << 20,
    UsableWhileStunned = 1 << 21,
    SingleTargetSpell = 1 << 22,
    StartPeriodicAtApply = 1 << 23,
    HasteAffectDuration = 1 << 24,
    UsableWhileFeared = 1 << 25,
    UsableWhileConfused = 1 << 26,
    CantTargetCrowdControlled = 1 << 27,
    CantBeReflected = 1 << 28,
    Passive = 1 << 29,

    Negative = NegativeEff0 | NegativeEff1 | NegativeEff2,
};


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
};

public enum SpellCastSource
{
    Player,
    Normal,
    Item,
    Passive,
    Pet,
    Aura,
    Spell,
};

public enum SpellRangeFlag
{
    Default,
    Melee,
    Ranged,
};

public enum SpellState
{
    None,
    Preparing,
    Casting,
    Finished,
    Idle,
    Delayed,
};

public enum SpellEffectHandleMode
{
    Launch,
    LaunchTarget,
    Hit,
    HitTarget,
};


public enum SpellSchools
{
    Normal,
    Holy,
    Fire,
    Nature,
    Frost,
    Shadow,
    Arcane,
};

[Flags]
public enum SpellSchoolMask
{
    None = 0,
    Normal = (1 << SpellSchools.Normal),
    Holy = (1 << SpellSchools.Holy),
    Fire = (1 << SpellSchools.Fire),
    Nature = (1 << SpellSchools.Nature),
    Frost = (1 << SpellSchools.Frost),
    Shadow = (1 << SpellSchools.Shadow),
    Arcane = (1 << SpellSchools.Arcane),

    Spell = (Fire | Nature | Frost | Shadow | Arcane),
    Magic = (Holy | Spell),
    All = (Normal | Magic),
};


public enum Powers
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
    Maelstrom1,
    Chi,
    Insanity,
    BurningEmbers,
    DemonicFury,
    ArcaneCharges,
    Fury,
    Pain,
    Health,
};

public enum SpellEffIndex
{
    Effect0 = 0,
    Effect1 = 1,
    Effect2 = 2,
    Effect3 = 3,
    Effect4 = 4,
    Effect5 = 5,
    Effect6 = 6,
    Effect7 = 7,
    Effect8 = 8,
    Effect9 = 9,
    Effect10 = 10,
    Effect11 = 11,
    Effect12 = 12,
    Effect13 = 13,
    Effect14 = 14,
    Effect15 = 15,
    Effect16 = 16,
    Effect17 = 17,
    Effect18 = 18,
    Effect19 = 19,
    Effect20 = 20,
    Effect21 = 21,
    Effect22 = 22,
    Effect23 = 23,
    Effect24 = 24,
    Effect25 = 25,
    Effect26 = 26,
    Effect27 = 27,
    Effect28 = 28,
    Effect29 = 29,
    Effect30 = 30,
    Effect31 = 31,
};

[Flags]
public enum TriggerCastFlags : uint
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

    FullMask = 0xFFFFFFFF,
};

public enum SpellCastResult
{
    Success,
    AlreadyAtFullHealth,
    AlreadyAtFullMana,
    AlreadyAtFullPower,
    AuraBounced,
    AutotrackInterrupted,
    BadImplicitTargets,
    BadTargets,
    CantBeCharmed,
    CantStealth,
    CantUntalent,
    CasterAurastate,
    CasterDead,
    Charmed,
    Confused,
    DontReport,
    EquippedItem,
    EquippedItemClass,
    EquippedItemClassMainhand,
    EquippedItemClassOffhand,
    Error,
    Falling,
    Immune,
    IncorrectArea,
    Interrupted,
    InterruptedCombat,
    LineOfSight,
    Moving,
    Nopath,
    NotBehind,
    NotHere,
    NotInfront,
    NotInControl,
    NotKnown,
    NotReady,
    NotShapeshift,
    NotStanding,
    NoChargesRemain,
    NoComboPoints,
    NoPower,
    NothingToDispel,
    NothingToSteal,
    OnlyStealthed,
    OutOfRange,
    Pacified,
    Rooted,
    Silenced,
    SpellInProgress,
    SpellUnavailable,
    Stunned,
    TargetsDead,
    TargetAurastate,
    TargetEnemy,
    TargetEnraged,
    TargetFriendly,
    TargetIsPlayer,
    TargetNotDead,
    TargetNotPlayer,
    TargetNoPockets,
    TargetNoWeapons,
    TargetNoRangedWeapons,
    TooClose,
    TryAgain,
    UnitNotBehind,
    UnitNotInfront,
    VisionObscured,
    DamageImmune,
    PreventedByMechanic,
    NotOnShapeshift,
    NotOnStealthed,
    NotOnDamageImmune,
    DisabledByAuraLabel,

    SpellCastOk = Success,
};


public enum DispelType
{
    None,
    Magic,
    Curse,
    Disease,
    Poison,
    Stealth,
    Invisibility,
    All,
    Enrage,
};

public enum AuraStateType
{
    None,
    Defense,
    Healthless20Percent,
    Berserking,
    Frozen,
    Judgement,
    Healthless35Percent,
    Conflagrate,
    Swiftmend,
    DeadlyPoison,
    Enrage,
    Bleeding,
    HealthAbove75Percent,
};

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
    MaxMechanic,
};

public enum TargetTypes
{
    TargetUnitCaster,
    TargetUnitNearbyEnemy,
    TargetUnitNearbyParty,
    TargetUnitNearbyAlly,
    TargetUnitTargetEnemy,
    TargetUnitSrcAreaEntry,
    TargetUnitDestAreaEntry,
    TargetUnitSrcAreaEnemy,
    TargetUnitDestAreaEnemy,
    TargetDestCaster,
    TargetUnitTargetAlly,
    TargetSrcCaster,
    TargetUnitConeEnemy24,
    TargetUnitTargetAny,
    TargetUnitSrcAreaAlly,
    TargetUnitDestAreaAlly,
    TargetUnitNearbyEntry,
    TargetDestCasterFrontRight,
    TargetDestCasterBackRight,
    TargetDestCasterBackLeft,
    TargetDestCasterFrontLeft,
    TargetUnitTargetChainhealAlly,
    TargetDestNearbyEntry,
    TargetDestCasterFront,
    TargetDestCasterBack,
    TargetDestCasterRight,
    TargetDestCasterLeft,
    TargetDestTargetEnemy,
    TargetUnitConeEnemy54,
    TargetDestCasterFrontLeap,
    TargetUnitConeAlly,
    TargetUnitConeEntry,
    TargetDestTargetAny,
    TargetDestTargetFront,
    TargetDestTargetBack,
    TargetDestTargetRight,
    TargetDestTargetLeft,
    TargetDestTargetFrontRight,
    TargetDestTargetBackRight,
    TargetDestTargetBackLeft,
    TargetDestTargetFrontLeft,
    TargetDestCasterRandom,
    TargetDestCasterRadius,
    TargetDestTargetRandom,
    TargetDestTargetRadius,
    TargetDestChannelTarget,
    TargetUnitChannelTarget,
    TargetDestDestFront,
    TargetDestDestBack,
    TargetDestDestRight,
    TargetDestDestLeft,
    TargetDestDestFrontRight,
    TargetDestDestBackRight,
    TargetDestDestBackLeft,
    TargetDestDestFrontLeft,
    TargetDestDestRandom,
    TargetDestDest,
    TargetDestTraj,
    TargetDestDestRadius,
    TargetDestChannelCaster,
};

public enum SpellDamageClass
{
    None,
    Magic,
    Melee,
    Ranged,
};

public enum SpellPreventionType
{
    NoActions,
    Silence,
    Pacify,
};

[Flags]
public enum SpellInterruptFlags
{
    None = 0,
    Movement = 1 << 0,
    PushBack = 1 << 1,
    Interrupt = 1 << 2,
    AbortOnDmg = 1 << 3,
};

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
};

public enum SpellEffectType
{
    None,
    Instakill,
    SchoolDamage,
    ApplyAura,
    Heal,
    Resurrect,
    AddExtraAttacks,
    Dodge,
    Evade,
    Parry,
    Block,
    PersistentAreaAura,
    Summon,
    Leap,
    WeaponPercentDamage,
    TriggerMissile,
    Dispel,
    Jump,
    JumpDest,
    Stealth,
    Detect,
    TriggerSpell,
    HealMaxHealth,
    InterruptCast,
    Pull,
    ScriptEffect,
    AddComboPoints,
    Charge6,
    KnockBack,
    DispelMechanic,
    ApplyAreaAuraFriend,
    ApplyAreaAuraEnemy,
    TriggerSpellWithValue,
    ApplyAreaAuraOwner,
    RemoveAura,
    TotalSpellEffects,
};

public enum DiminishingLevels
{
    Level1,
    Level2,
    Level3,
    Level4,
    Immune,
};

public enum DiminishingGroup
{
    None,
    Root,
    Stun,
    Incapacitate,
    Disorient,
    Silence,
    AoeKnockback,
};

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
};

public enum DamageEffectType
{
    DirectDamage = 0,                   // used for normal weapon damage (not for class abilities or spells)
    SpellDirectDamage = 1,              // spell/class abilities damage
    Dot = 2,
    Heal = 3,
    Nodamage = 4,                       // used also in case when damage applied to health but not applied to spell channelInterruptFlags/etc
    SelfDamage = 5
};