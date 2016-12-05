using System;

[Flags]
public enum SpellCastTargetFlags
{
    NONE = 0x00000000,
    UNIT = 0x00000002,               // id
    UNIT_RAID = 0x00000004,          // not sent, used to validate target (if raid member)
    UNIT_PARTY = 0x00000008,         // not sent, used to validate target (if party member)
    ITEM = 0x00000010,               // id
    SOURCE_LOCATION = 0x00000020,    // id, 3 float
    DEST_LOCATION = 0x00000040,      // id, 3 float
    UNIT_ENEMY = 0x00000080,         // not sent, used to validate target (if enemy)
    UNIT_ALLY = 0x00000100,          // not sent, used to validate target (if ally)
    CORPSE_ENEMY = 0x00000200,       // id
    UNIT_DEAD = 0x00000400,          // not sent, used to validate target (if dead creature)
    GAMEOBJECT = 0x00000800,         // id, used with TARGET_GAMEOBJECT_TARGET
    STRING = 0x00002000,             // string
    CORPSE_ALLY = 0x00008000,        // id
    UNIT_MINIPET = 0x00010000,       // id, used to validate target (if non combat pet)
    GLYPH_SLOT = 0x00020000,         // used in glyph spells

    UNIT_MASK = UNIT | UNIT_RAID | UNIT_PARTY
        | UNIT_ENEMY | UNIT_ALLY | UNIT_DEAD | UNIT_MINIPET,
    CORPSE_MASK = CORPSE_ALLY | CORPSE_ENEMY,
};

public enum SpellTargetSelectionCategories
{
    NYI,
    DEFAULT,
    CHANNEL,
    NEARBY,
    CONE,
    AREA
};

public enum SpellTargetReferenceTypes
{
    NONE,
    CASTER,
    TARGET,
    LAST,
    SRC,
    DEST
};

public enum SpellTargetObjectTypes
{
    NONE = 0,
    SRC,
    DEST,
    UNIT,
    UNIT_AND_DEST,
    GOBJ,
    GOBJ_ITEM,
    ITEM,
    CORPSE,
    CORPSE_ENEMY,
    CORPSE_ALLY
};

public enum SpellTargetCheckTypes
{
    DEFAULT,
    ENTRY,
    ENEMY,
    ALLY,
    PARTY,
    RAID,
    RAID_CLASS,
    PASSENGER
};

public enum SpellTargetDirectionTypes
{
    NONE,
    FRONT,
    BACK,
    RIGHT,
    LEFT,
    FRONT_RIGHT,
    BACK_RIGHT,
    BACK_LEFT,
    FRONT_LEFT,
    RANDOM,
    ENTRY
};

enum SpellEffectImplicitTargetTypes
{
    NONE = 0,
    EXPLICIT,
    CASTER
};

public enum SpellSpecificType
{
    NORMAL = 0,
    SEAL = 1,
    AURA = 3,
    STING = 4,
    CURSE = 5,
    ASPECT = 6,
    TRACKER = 7,
    WARLOCK_ARMOR = 8,
    MAGE_ARMOR = 9,
    ELEMENTAL_SHIELD = 10,
    MAGE_POLYMORPH = 11,
    JUDGEMENT = 13,
    WARLOCK_CORRUPTION = 17,
    FOOD = 19,
    DRINK = 20,
    FOOD_AND_DRINK = 21,
    PRESENCE = 22,
    CHARM = 23,
    SCROLL = 24,
    MAGE_ARCANE_BRILLANCE = 25,
    WARRIOR_ENRAGE = 26,
    PRIEST_DIVINE_SPIRIT = 27,
    HAND = 28,
    PHASE = 29,
    BANE = 30
};

public enum SpellCustomAttributes
{
    ENCHANT_PROC = 0x00000001,
    CONE_BACK = 0x00000002,
    CONE_LINE = 0x00000004,
    SHARE_DAMAGE = 0x00000008,
    NO_INITIAL_THREAT = 0x00000010,
    IS_TALENT = 0x00000020,
    DONT_BREAK_STEALTH = 0x00000040,
    DIRECT_DAMAGE = 0x00000100,
    CHARGE = 0x00000200,
    PICKPOCKET = 0x00000400,
    NEGATIVE_EFF0 = 0x00001000,
    NEGATIVE_EFF1 = 0x00002000,
    NEGATIVE_EFF2 = 0x00004000,
    IGNORE_ARMOR = 0x00008000,
    REQ_TARGET_FACING_CASTER = 0x00010000,
    REQ_CASTER_BEHIND_TARGET = 0x00020000,
    ALLOW_INFLIGHT_TARGET = 0x00040000,
    NEEDS_AMMO_DATA = 0x00080000,

    NEGATIVE = NEGATIVE_EFF0 | NEGATIVE_EFF1 | NEGATIVE_EFF2
};

public struct SpellRadiusEntry
{
    public int Id;
    public float Radius;
    public float RadiusPerLevel;
    public float RadiusMin;
    public float RadiusMax;
};

public struct SpellRangeEntry
{
    public int Id;
    public float MinRangeHostile;
    public float MinRangeFriend;
    public float MaxRangeHostile;
    public float MaxRangeFriend;
    public int Flags;
};

public struct SpellCastTimesEntry
{
    int Id;
    int CastTime;
    int MinCastTime;
    int CastTimePerLevel;
};

public struct SpellProcsPerMinuteModEntry
{
    public int Id;
    public float Coeff;
    public int Param;
    public int Type;
    public int SpellProcsPerMinuteID;
};

public struct SpellCooldownsEntry
{
    public int Id;
    public int SpellId;
    public int CategoryRecoveryTime;
    public int RecoveryTime;
    public int StartRecoveryTime;
    public int DifficultyId;
};

public struct SpellDurationEntry
{
    public int ID;
    public int Duration;
    public int MaxDuration;
    public int DurationPerLevel;
};

public struct SpellPowerEntry
{
    public int SpellId;
    public int ManaCost;
    float ManaCostPercentage;
    float ManaCostPercentagePerSecond;
    public int RequiredAura;
    float HealthCostPercentage;
    public int PowerIndex;
    public int PowerType;
    public int Id;
    public int ManaCostPerLevel;
    public int ManaCostPerSecond;
    public int ManaCostAdditional;
    
    public int PowerDisplayId;
    public int UnitPowerBarId;
};

public enum SpellCastFlags
{
    NONE =  0x00000000,
    PENDING = 0x00000001,
    HAS_TRAJECTORY = 0x00000002,
    PROJECTILE = 0x00000020,
    POWER_LEFT_SELF = 0x00000800,
    ADJUST_MISSILE = 0x00020000,
    NO_GCD = 0x00040000,
    VISUAL_CHAIN = 0x00080000,
    IMMUNITY = 0x04000000,
};

public enum SpellCastSource
{
    PLAYER = 0,
    NORMAL = 3,
    ITEM = 4,
    PASSIVE = 7,
    PET = 9,
    AURA = 13,
    SPELL = 16,
};

public enum SpellRangeFlag
{
    DEFAULT = 0,
    MELEE = 1,
    RANGED = 2,
};


public struct SpellValue
{
    int[] EffectBasePoints;
    int MaxAffectedTargets;
    float RadiusMod;
    int AuraStackAmount;
};

public enum SpellState
{
    NONE = 0,
    PREPARING = 1,
    CASTING = 2,
    FINISHED = 3,
    IDLE = 4,
    DELAYED = 5
};

public enum SpellEffectHandleMode
{
    LAUNCH,
    LAUNCH_TARGET,
    HIT,
    HIT_TARGET
};


public enum SpellSchools
{
    NORMAL = 0,
    HOLY = 1,
    FIRE = 2,
    NATURE = 3,
    FROST = 4,
    SHADOW = 5,
    ARCANE = 6
};

[Flags]
public enum SpellSchoolMask
{
    NONE = 0x00,
    NORMAL = (1 << SpellSchools.NORMAL),
    HOLY = (1 << SpellSchools.HOLY),
    FIRE = (1 << SpellSchools.FIRE),
    NATURE = (1 << SpellSchools.NATURE),
    FROST = (1 << SpellSchools.FROST),
    SHADOW = (1 << SpellSchools.SHADOW),
    ARCANE = (1 << SpellSchools.ARCANE),

    // 124, not include normal and holy damage
    SPELL = (FIRE | NATURE | FROST | SHADOW | ARCANE),
    // 126
    MAGIC = (HOLY | SPELL),
    // 127
    ALL = (NORMAL | MAGIC)
};

public struct PowerCostData
{
    Powers Power;
    int Amount;
};

public enum Powers
{
    MANA = 0,
    RAGE = 1,
    FOCUS = 2,
    ENERGY = 3,
    COMBO_POINTS = 4,
    RUNES = 5,
    RUNIC_POWER = 6,
    SOUL_SHARDS = 7,
    LUNAR_POWER = 8,
    HOLY_POWER = 9,
    ALTERNATE_POWER = 10,
    MAELSTROM = 11,
    CHI = 12,
    INSANITY = 13,
    BURNING_EMBERS = 14,
    DEMONIC_FURY = 15,
    ARCANE_CHARGES = 16,
    FURY = 17,
    PAIN = 18,
    MAX_POWERS = 19,
    ALL = 127,
    HEALTH = 0xFE,
};