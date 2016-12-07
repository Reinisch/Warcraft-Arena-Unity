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

public enum SpellEffIndex
{
    EFFECT_0 = 0,
    EFFECT_1 = 1,
    EFFECT_2 = 2,
    EFFECT_3 = 3,
    EFFECT_4 = 4,
    EFFECT_5 = 5,
    EFFECT_6 = 6,
    EFFECT_7 = 7,
    EFFECT_8 = 8,
    EFFECT_9 = 9,
    EFFECT_10 = 10,
    EFFECT_11 = 11,
    EFFECT_12 = 12,
    EFFECT_13 = 13,
    EFFECT_14 = 14,
    EFFECT_15 = 15,
    EFFECT_16 = 16,
    EFFECT_17 = 17,
    EFFECT_18 = 18,
    EFFECT_19 = 19,
    EFFECT_20 = 20,
    EFFECT_21 = 21,
    EFFECT_22 = 22,
    EFFECT_23 = 23,
    EFFECT_24 = 24,
    EFFECT_25 = 25,
    EFFECT_26 = 26,
    EFFECT_27 = 27,
    EFFECT_28 = 28,
    EFFECT_29 = 29,
    EFFECT_30 = 30,
    EFFECT_31 = 31
};

public enum TriggerCastFlags : uint
{
    NONE = 0x00000000,   //! Not triggered
    IGNORE_GCD = 0x00000001,   //! Will ignore GCD
    IGNORE_SPELL_AND_CATEGORY_CD = 0x00000002,   //! Will ignore Spell and Category cooldowns
    IGNORE_POWER_AND_REAGENT_COST = 0x00000004,   //! Will ignore power and reagent cost
    IGNORE_CAST_ITEM = 0x00000008,   //! Will not take away cast item or update related achievement criteria
    IGNORE_AURA_SCALING = 0x00000010,   //! Will ignore aura scaling
    IGNORE_CAST_IN_PROGRESS = 0x00000020,   //! Will not check if a current cast is in progress
    IGNORE_COMBO_POINTS = 0x00000040,   //! Will ignore combo point requirement
    CAST_DIRECTLY = 0x00000080,   //! In Spell::prepare, will be cast directly without setting containers for executed spell
    IGNORE_AURA_INTERRUPT_FLAGS = 0x00000100,   //! Will ignore interruptible aura's at cast
    IGNORE_SET_FACING = 0x00000200,   //! Will not adjust facing to target (if any)
    IGNORE_SHAPESHIFT = 0x00000400,   //! Will ignore shapeshift checks
    IGNORE_CASTER_AURASTATE = 0x00000800,   //! Will ignore caster aura states including combat requirements and death state
    IGNORE_CASTER_MOUNTED_OR_ON_VEHICLE = 0x00002000,   //! Will ignore mounted/on vehicle restrictions
    IGNORE_CASTER_AURAS = 0x00010000,   //! Will ignore caster aura restrictions or requirements
    DISALLOW_PROC_EVENTS = 0x00020000,   //! Disallows proc events from triggered spell (default)
    DONT_REPORT_CAST_ERROR = 0x00040000,   //! Will return SPELL_FAILED_DONT_REPORT in CheckCast functions
    IGNORE_EQUIPPED_ITEM_REQUIREMENT = 0x00080000,   //! Will ignore equipped item requirements
    IGNORE_TARGET_CHECK = 0x00100000,   //! Will ignore most target checks (mostly DBC target checks)
    FULL_MASK = 0xFFFFFFFF
};

public enum SpellCastResult
{
    SUCCESS = 0,
    AFFECTING_COMBAT = 1,
    ALREADY_AT_FULL_HEALTH = 2,
    ALREADY_AT_FULL_MANA = 3,
    ALREADY_AT_FULL_POWER = 4,
    ALREADY_BEING_TAMED = 5,
    ALREADY_HAVE_CHARM = 6,
    ALREADY_HAVE_SUMMON = 7,
    ALREADY_HAVE_PET = 8,
    ALREADY_OPEN = 9,
    AURA_BOUNCED = 10,
    AUTOTRACK_INTERRUPTED = 11,
    BAD_IMPLICIT_TARGETS = 12,
    BAD_TARGETS = 13,
    PVP_TARGET_WHILE_UNFLAGGED = 14,
    CANT_BE_CHARMED = 15,
    CANT_BE_DISENCHANTED = 16,
    CANT_BE_DISENCHANTED_SKILL = 17,
    CANT_BE_MILLED = 18,
    CANT_BE_PROSPECTED = 19,
    CANT_CAST_ON_TAPPED = 20,
    CANT_DUEL_WHILE_INVISIBLE = 21,
    CANT_DUEL_WHILE_STEALTHED = 22,
    CANT_STEALTH = 23,
    CANT_UNTALENT = 24,
    CASTER_AURASTATE = 25,
    CASTER_DEAD = 26,
    CHARMED = 27,
    CHEST_IN_USE = 28,
    CONFUSED = 29,
    DONT_REPORT = 30,
    EQUIPPED_ITEM = 31,
    EQUIPPED_ITEM_CLASS = 32,
    EQUIPPED_ITEM_CLASS_MAINHAND = 33,
    EQUIPPED_ITEM_CLASS_OFFHAND = 34,
    ERROR = 35,
    FALLING = 36,
    FIZZLE = 37,
    FLEEING = 38,
    FOOD_LOWLEVEL = 39,
    GARRISON_NOT_OWNED = 40,
    GARRISON_OWNED = 41,
    GARRISON_MAX_LEVEL = 42,
    GARRISON_NOT_UPGRADEABLE = 43,
    GARRISON_FOLLOWER_ON_MISSION = 44,
    GARRISON_FOLLOWER_IN_BUILDING = 45,
    GARRISON_FOLLOWER_MAX_LEVEL = 46,
    GARRISON_FOLLOWER_MAX_ITEM_LEVEL = 47,
    GARRISON_FOLLOWER_MAX_QUALITY = 48,
    GARRISON_FOLLOWER_NOT_MAX_LEVEL = 49,
    GARRISON_FOLLOWER_HAS_ABILITY = 50,
    GARRISON_FOLLOWER_HAS_SINGLE_MISSION_ABILITY = 51,
    GARRISON_FOLLOWER_REQUIRES_EPIC = 52,
    GARRISON_MISSION_NOT_IN_PROGRESS = 53,
    GARRISON_MISSION_COMPLETE = 54,
    GARRISON_NO_MISSIONS_AVAILABLE = 55,
    HIGHLEVEL = 56,
    HUNGER_SATIATED = 57,
    IMMUNE = 58,
    INCORRECT_AREA = 59,
    INTERRUPTED = 60,
    INTERRUPTED_COMBAT = 61,
    ITEM_ALREADY_ENCHANTED = 62,
    ITEM_GONE = 63,
    ITEM_NOT_FOUND = 64,
    ITEM_NOT_READY = 65,
    LEVEL_REQUIREMENT = 66,
    LINE_OF_SIGHT = 67,
    LOWLEVEL = 68,
    LOW_CASTLEVEL = 69,
    MAINHAND_EMPTY = 70,
    MOVING = 71,
    NEED_AMMO = 72,
    NEED_AMMO_POUCH = 73,
    NEED_EXOTIC_AMMO = 74,
    NEED_MORE_ITEMS = 75,
    NOPATH = 76,
    NOT_BEHIND = 77,
    NOT_FISHABLE = 78,
    NOT_FLYING = 79,
    NOT_HERE = 80,
    NOT_INFRONT = 81,
    NOT_IN_CONTROL = 82,
    NOT_KNOWN = 83,
    NOT_MOUNTED = 84,
    NOT_ON_TAXI = 85,
    NOT_ON_TRANSPORT = 86,
    NOT_READY = 87,
    NOT_SHAPESHIFT = 88,
    NOT_STANDING = 89,
    NOT_TRADEABLE = 90,
    NOT_TRADING = 91,
    NOT_UNSHEATHED = 92,
    NOT_WHILE_GHOST = 93,
    NOT_WHILE_LOOTING = 94,
    NO_AMMO = 95,
    NO_CHARGES_REMAIN = 96,
    NO_COMBO_POINTS = 97,
    NO_DUELING = 98,
    NO_ENDURANCE = 99,
    NO_FISH = 100,
    NO_ITEMS_WHILE_SHAPESHIFTED = 101,
    NO_MOUNTS_ALLOWED = 102,
    NO_PET = 103,
    NO_POWER = 104,
    NOTHING_TO_DISPEL = 105,
    NOTHING_TO_STEAL = 106,
    ONLY_ABOVEWATER = 107,
    ONLY_INDOORS = 108,
    ONLY_MOUNTED = 109,
    ONLY_OUTDOORS = 110,
    ONLY_SHAPESHIFT = 111,
    ONLY_STEALTHED = 112,
    ONLY_UNDERWATER = 113,
    OUT_OF_RANGE = 114,
    PACIFIED = 115,
    POSSESSED = 116,
    REAGENTS = 117,
    REQUIRES_AREA = 118,
    REQUIRES_SPELL_FOCUS = 119,
    ROOTED = 120,
    SILENCED = 121,
    SPELL_IN_PROGRESS = 122,
    SPELL_LEARNED = 123,
    SPELL_UNAVAILABLE = 124,
    STUNNED = 125,
    TARGETS_DEAD = 126,
    TARGET_AFFECTING_COMBAT = 127,
    TARGET_AURASTATE = 128,
    TARGET_DUELING = 129,
    TARGET_ENEMY = 130,
    TARGET_ENRAGED = 131,
    TARGET_FRIENDLY = 132,
    TARGET_IN_COMBAT = 133,
    TARGET_IN_PET_BATTLE = 134,
    TARGET_IS_PLAYER = 135,
    TARGET_IS_PLAYER_CONTROLLED = 136,
    TARGET_NOT_DEAD = 137,
    TARGET_NOT_IN_PARTY = 138,
    TARGET_NOT_LOOTED = 139,
    TARGET_NOT_PLAYER = 140,
    TARGET_NO_POCKETS = 141,
    TARGET_NO_WEAPONS = 142,
    TARGET_NO_RANGED_WEAPONS = 143,
    TARGET_UNSKINNABLE = 144,
    THIRST_SATIATED = 145,
    TOO_CLOSE = 146,
    TOO_MANY_OF_ITEM = 147,
    TOTEM_CATEGORY = 148,
    TOTEMS = 149,
    TRY_AGAIN = 150,
    UNIT_NOT_BEHIND = 151,
    UNIT_NOT_INFRONT = 152,
    VISION_OBSCURED = 153,
    WRONG_PET_FOOD = 154,
    NOT_WHILE_FATIGUED = 155,
    TARGET_NOT_IN_INSTANCE = 156,
    NOT_WHILE_TRADING = 157,
    TARGET_NOT_IN_RAID = 158,
    TARGET_FREEFORALL = 159,
    NO_EDIBLE_CORPSES = 160,
    ONLY_BATTLEGROUNDS = 161,
    TARGET_NOT_GHOST = 162,
    TRANSFORM_UNUSABLE = 163,
    WRONG_WEATHER = 164,
    DAMAGE_IMMUNE = 165,
    PREVENTED_BY_MECHANIC = 166,
    PLAY_TIME = 167,
    REPUTATION = 168,
    MIN_SKILL = 169,
    NOT_IN_RATED_BATTLEGROUND = 170,
    NOT_ON_SHAPESHIFT = 171,
    NOT_ON_STEALTHED = 172,
    NOT_ON_DAMAGE_IMMUNE = 173,
    NOT_ON_MOUNTED = 174,
    TOO_SHALLOW = 175,
    TARGET_NOT_IN_SANCTUARY = 176,
    TARGET_IS_TRIVIAL = 177,
    BM_OR_INVISGOD = 178,
    GROUND_MOUNT_NOT_ALLOWED = 179,
    FLOATING_MOUNT_NOT_ALLOWED = 180,
    UNDERWATER_MOUNT_NOT_ALLOWED = 181,
    FLYING_MOUNT_NOT_ALLOWED = 182,
    APPRENTICE_RIDING_REQUIREMENT = 183,
    JOURNEYMAN_RIDING_REQUIREMENT = 184,
    EXPERT_RIDING_REQUIREMENT = 185,
    ARTISAN_RIDING_REQUIREMENT = 186,
    MASTER_RIDING_REQUIREMENT = 187,
    COLD_RIDING_REQUIREMENT = 188,
    FLIGHT_MASTER_RIDING_REQUIREMENT = 189,
    CS_RIDING_REQUIREMENT = 190,
    PANDA_RIDING_REQUIREMENT = 191,
    DRAENOR_RIDING_REQUIREMENT = 192,
    MOUNT_NO_FLOAT_HERE = 193,
    MOUNT_NO_UNDERWATER_HERE = 194,
    MOUNT_ABOVE_WATER_HERE = 195,
    MOUNT_COLLECTED_ON_OTHER_CHAR = 196,
    NOT_IDLE = 197,
    NOT_INACTIVE = 198,
    PARTIAL_PLAYTIME = 199,
    NO_PLAYTIME = 200,
    NOT_IN_BATTLEGROUND = 201,
    NOT_IN_RAID_INSTANCE = 202,
    ONLY_IN_ARENA = 203,
    TARGET_LOCKED_TO_RAID_INSTANCE = 204,
    ON_USE_ENCHANT = 205,
    NOT_ON_GROUND = 206,
    CUSTOM_ERROR = 207,
    CANT_DO_THAT_RIGHT_NOW = 208,
    TOO_MANY_SOCKETS = 209,
    INVALID_GLYPH = 210,
    UNIQUE_GLYPH = 211,
    GLYPH_SOCKET_LOCKED = 212,
    GLYPH_EXCLUSIVE_CATEGORY = 213,
    GLYPH_INVALID_SPEC = 214,
    GLYPH_NO_SPEC = 215,
    NO_VALID_TARGETS = 216,
    ITEM_AT_MAX_CHARGES = 217,
    NOT_IN_BARBERSHOP = 218,
    FISHING_TOO_LOW = 219,
    ITEM_ENCHANT_TRADE_WINDOW = 220,
    SUMMON_PENDING = 221,
    MAX_SOCKETS = 222,
    PET_CAN_RENAME = 223,
    TARGET_CANNOT_BE_RESURRECTED = 224,
    TARGET_HAS_RESURRECT_PENDING = 225,
    NO_ACTIONS = 226,
    CURRENCY_WEIGHT_MISMATCH = 227,
    WEIGHT_NOT_ENOUGH = 228,
    WEIGHT_TOO_MUCH = 229,
    NO_VACANT_SEAT = 230,
    NO_LIQUID = 231,
    ONLY_NOT_SWIMMING = 232,
    BY_NOT_MOVING = 233,
    IN_COMBAT_RES_LIMIT_REACHED = 234,
    NOT_IN_ARENA = 235,
    TARGET_NOT_GROUNDED = 236,
    EXCEEDED_WEEKLY_USAGE = 237,
    NOT_IN_LFG_DUNGEON = 238,
    BAD_TARGET_FILTER = 239,
    NOT_ENOUGH_TARGETS = 240,
    NO_SPEC = 241,
    CANT_ADD_BATTLE_PET = 242,
    CANT_UPGRADE_BATTLE_PET = 243,
    WRONG_BATTLE_PET_TYPE = 244,
    NO_DUNGEON_ENCOUNTER = 245,
    NO_TELEPORT_FROM_DUNGEON = 246,
    MAX_LEVEL_TOO_LOW = 247,
    CANT_REPLACE_ITEM_BONUS = 248,
    GRANT_PET_LEVEL_FAIL = 249,
    SKILL_LINE_NOT_KNOWN = 250,
    BLUEPRINT_KNOWN = 251,
    FOLLOWER_KNOWN = 252,
    CANT_OVERRIDE_ENCHANT_VISUAL = 253,
    ITEM_NOT_A_WEAPON = 254,
    SAME_ENCHANT_VISUAL = 255,
    TOY_USE_LIMIT_REACHED = 256,
    TOY_ALREADY_KNOWN = 257,
    SHIPMENTS_FULL = 258,
    NO_SHIPMENTS_FOR_CONTAINER = 259,
    NO_BUILDING_FOR_SHIPMENT = 260,
    NOT_ENOUGH_SHIPMENTS_FOR_CONTAINER = 261,
    HAS_MISSION = 262,
    BUILDING_ACTIVATE_NOT_READY = 263,
    NOT_SOULBOUND = 264,
    RIDING_VEHICLE = 265,
    VETERAN_TRIAL_ABOVE_SKILL_RANK_MAX = 266,
    NOT_WHILE_MERCENARY = 267,
    SPEC_DISABLED = 268,
    CANT_BE_OBLITERATED = 269,
    FOLLOWER_CLASS_SPEC_CAP = 270,
    TRANSPORT_NOT_READY = 271,
    TRANSMOG_SET_ALREADY_KNOWN = 272,
    DISABLED_BY_AURA_LABEL = 273,
    DISABLED_BY_MAX_USABLE_LEVEL = 274,
    SPELL_ALREADY_KNOWN = 275,
    MUST_KNOW_SUPERCEDING_SPELL = 276,
    YOU_CANNOT_USE_THAT_IN_PVP_INSTANCE = 277,
    NO_ARTIFACT_EQUIPPED = 278,
    WRONG_ARTIFACT_EQUIPPED = 279,
    TARGET_IS_UNTARGETABLE_BY_ANYONE = 280,
    SPELL_EFFECT_FAILED = 281,
    UNKNOWN = 282,

    // ok cast value - here in case a future version removes SUCCESS and we need to use a custom value (not sent to client either way)
    SPELL_CAST_OK = SUCCESS
};