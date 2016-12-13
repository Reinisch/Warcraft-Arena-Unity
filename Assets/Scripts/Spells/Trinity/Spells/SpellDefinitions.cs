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

public enum TargetSelections
{
    NYI,
    DEFAULT,
    CHANNEL,
    NEARBY,
    CONE,
    AREA
};

public enum TargetReferences
{
    NONE,
    CASTER,
    TARGET,
    LAST,
    SRC,
    DEST
};

public enum TargetObjects
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

public enum TargetChecks
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

public enum TargetDirections
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

public enum SpellAttributes : uint
{
    NONE =                          0x00000000,
    CONE_BACK =                     0x00000001,
    CONE_LINE =                     0x00000002,
    CHARGE =                        0x00000004,
    NEGATIVE_EFF0 =                 0x00000008,
    NEGATIVE_EFF1 =                 0x00000010,
    NEGATIVE_EFF2 =                 0x00000020,
    IGNORE_ARMOR =                  0x00000040,
    REQ_TARGET_FACING_CASTER =      0x00000080,
    REQ_CASTER_BEHIND_TARGET =      0x00000100,
    CANT_CRIT =                     0x00000200,
    TRIGGERED_CAN_TRIGGER_PROC =    0x00000400,
    STACK_FOR_DIFF_CASTERS =        0x00000800,
    ONLY_TARGET_PLAYERS =           0x00001000,
    IGNORE_HIT_RESULT =             0x00002000,
    IGNORE_RESISTANCES =            0x00004000,
    PROC_ONLY_ON_CASTER =           0x00008000,
    NOT_STEALABLE =                 0x00010000,
    CAN_CAST_WHILE_CASTING =        0x00020000,
    FIXED_DAMAGE =                  0x00040000,
    TRIGGER_ACTIVATE =              0x00080000,
    DAMAGE_DOESNT_BREAK_AURAS =     0x00100000,
    USABLE_WHILE_STUNNED =          0x00200000,
    SINGLE_TARGET_SPELL =           0x00400000,
    START_PERIODIC_AT_APPLY =       0x00800000,
    HASTE_AFFECT_DURATION =         0x01000000,
    USABLE_WHILE_FEARED =           0x02000000,
    USABLE_WHILE_CONFUSED =         0x04000000,
    CANT_TARGET_CROWD_CONTROLLED =  0x08000000,
    CANT_BE_REFLECTED =             0x10000000,
    PASSIVE =                       0x20000000,

    NEGATIVE = NEGATIVE_EFF0 | NEGATIVE_EFF1 | NEGATIVE_EFF2,

    ALL =                           0xFFFFFFFF,
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
    public int[] EffectBasePoints;
    public int MaxAffectedTargets;
    public float RadiusMod;
    public int AuraStackAmount;

    public SpellValue(TrinitySpellInfo spellInfo)
    {
        EffectBasePoints = new int[SpellHelper.MaxSpellEffects];

        for (int i = 0; i < spellInfo.SpellEffectInfos.Count; i++)
            if (spellInfo.SpellEffectInfos[i] != null)
                EffectBasePoints[spellInfo.SpellEffectInfos[i].EffectIndex] = spellInfo.SpellEffectInfos[i].BasePoints;

        MaxAffectedTargets = spellInfo.MaxAffectedTargets;
        RadiusMod = 1.0f;
        AuraStackAmount = 1;
    }
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


public enum DispelType
{
    NONE = 0,
    MAGIC = 1,
    CURSE = 2,
    DISEASE = 3,
    POISON = 4,
    STEALTH = 5,
    INVISIBILITY = 6,
    ALL = 7,
    ENRAGE = 9,
};

public enum AuraStateType
{
    NONE = 0,
    DEFENSE = 1,
    HEALTHLESS_20_PERCENT = 2,
    BERSERKING = 3,
    FROZEN = 4,
    JUDGEMENT = 5,
    WARRIOR_VICTORY_RUSH = 10,
    FAERIE_FIRE = 12,
    HEALTHLESS_35_PERCENT = 13,
    CONFLAGRATE = 14,
    SWIFTMEND = 15,
    DEADLY_POISON = 16,
    ENRAGE = 17,
    BLEEDING = 18,
    HEALTH_ABOVE_75_PERCENT = 23 
};

public enum Mechanics
{
    NONE = 0,
    CHARM = 1,
    DISORIENTED = 2,
    DISARM = 3,
    DISTRACT = 4,
    FEAR = 5,
    GRIP = 6,
    ROOT = 7,
    SLOW_ATTACK = 8,
    SILENCE = 9,
    SLEEP = 10,
    SNARE = 11,
    STUN = 12,
    FREEZE = 13,
    KNOCKOUT = 14,
    BLEED = 15,
    BANDAGE = 16,
    POLYMORPH = 17,
    BANISH = 18,
    SHIELD = 19,
    SHACKLE = 20,
    MOUNT = 21,
    INFECTED = 22,
    TURN = 23,
    HORROR = 24,
    INVULNERABILITY = 25,
    INTERRUPT = 26,
    DAZE = 27,
    DISCOVERY = 28,
    IMMUNE_SHIELD = 29,
    SAPPED = 30,
    ENRAGED = 31,
    WOUNDED = 32,
    MAX_MECHANIC = 33
};

public enum TargetTypes
{
    TARGET_UNIT_CASTER = 1,
    TARGET_UNIT_NEARBY_ENEMY = 2,
    TARGET_UNIT_NEARBY_PARTY = 3,
    TARGET_UNIT_NEARBY_ALLY = 4,
    TARGET_UNIT_PET = 5,
    TARGET_UNIT_TARGET_ENEMY = 6,
    TARGET_UNIT_SRC_AREA_ENTRY = 7,
    TARGET_UNIT_DEST_AREA_ENTRY = 8,
    TARGET_DEST_HOME = 9,
    TARGET_UNIT_SRC_AREA_UNK_11 = 11,
    TARGET_UNIT_SRC_AREA_ENEMY = 15,
    TARGET_UNIT_DEST_AREA_ENEMY = 16,
    TARGET_DEST_DB = 17,
    TARGET_DEST_CASTER = 18,
    TARGET_UNIT_CASTER_AREA_PARTY = 20,
    TARGET_UNIT_TARGET_ALLY = 21,
    TARGET_SRC_CASTER = 22,
    TARGET_GAMEOBJECT_TARGET = 23,
    TARGET_UNIT_CONE_ENEMY_24 = 24,
    TARGET_UNIT_TARGET_ANY = 25,
    TARGET_GAMEOBJECT_ITEM_TARGET = 26,
    TARGET_UNIT_MASTER = 27,
    TARGET_DEST_DYNOBJ_ENEMY = 28,
    TARGET_DEST_DYNOBJ_ALLY = 29,
    TARGET_UNIT_SRC_AREA_ALLY = 30,
    TARGET_UNIT_DEST_AREA_ALLY = 31,
    TARGET_DEST_CASTER_SUMMON = 32, // front left, doesn't use radius
    TARGET_UNIT_SRC_AREA_PARTY = 33,
    TARGET_UNIT_DEST_AREA_PARTY = 34,
    TARGET_UNIT_TARGET_PARTY = 35,
    TARGET_DEST_CASTER_UNK_36 = 36,
    TARGET_UNIT_LASTTARGET_AREA_PARTY = 37,
    TARGET_UNIT_NEARBY_ENTRY = 38,
    TARGET_DEST_CASTER_FISHING = 39,
    TARGET_GAMEOBJECT_NEARBY_ENTRY = 40,
    TARGET_DEST_CASTER_FRONT_RIGHT = 41,
    TARGET_DEST_CASTER_BACK_RIGHT = 42,
    TARGET_DEST_CASTER_BACK_LEFT = 43,
    TARGET_DEST_CASTER_FRONT_LEFT = 44,
    TARGET_UNIT_TARGET_CHAINHEAL_ALLY = 45,
    TARGET_DEST_NEARBY_ENTRY = 46,
    TARGET_DEST_CASTER_FRONT = 47,
    TARGET_DEST_CASTER_BACK = 48,
    TARGET_DEST_CASTER_RIGHT = 49,
    TARGET_DEST_CASTER_LEFT = 50,
    TARGET_GAMEOBJECT_SRC_AREA = 51,
    TARGET_GAMEOBJECT_DEST_AREA = 52,
    TARGET_DEST_TARGET_ENEMY = 53,
    TARGET_UNIT_CONE_ENEMY_54 = 54,
    TARGET_DEST_CASTER_FRONT_LEAP = 55, // for a leap spell
    TARGET_UNIT_CASTER_AREA_RAID = 56,
    TARGET_UNIT_TARGET_RAID = 57,
    TARGET_UNIT_NEARBY_RAID = 58,
    TARGET_UNIT_CONE_ALLY = 59,
    TARGET_UNIT_CONE_ENTRY = 60,
    TARGET_UNIT_TARGET_AREA_RAID_CLASS = 61,
    TARGET_UNK_62 = 62,
    TARGET_DEST_TARGET_ANY = 63,
    TARGET_DEST_TARGET_FRONT = 64,
    TARGET_DEST_TARGET_BACK = 65,
    TARGET_DEST_TARGET_RIGHT = 66,
    TARGET_DEST_TARGET_LEFT = 67,
    TARGET_DEST_TARGET_FRONT_RIGHT = 68,
    TARGET_DEST_TARGET_BACK_RIGHT = 69,
    TARGET_DEST_TARGET_BACK_LEFT = 70,
    TARGET_DEST_TARGET_FRONT_LEFT = 71,
    TARGET_DEST_CASTER_RANDOM = 72,
    TARGET_DEST_CASTER_RADIUS = 73,
    TARGET_DEST_TARGET_RANDOM = 74,
    TARGET_DEST_TARGET_RADIUS = 75,
    TARGET_DEST_CHANNEL_TARGET = 76,
    TARGET_UNIT_CHANNEL_TARGET = 77,
    TARGET_DEST_DEST_FRONT = 78,
    TARGET_DEST_DEST_BACK = 79,
    TARGET_DEST_DEST_RIGHT = 80,
    TARGET_DEST_DEST_LEFT = 81,
    TARGET_DEST_DEST_FRONT_RIGHT = 82,
    TARGET_DEST_DEST_BACK_RIGHT = 83,
    TARGET_DEST_DEST_BACK_LEFT = 84,
    TARGET_DEST_DEST_FRONT_LEFT = 85,
    TARGET_DEST_DEST_RANDOM = 86,
    TARGET_DEST_DEST = 87,
    TARGET_DEST_DYNOBJ_NONE = 88,
    TARGET_DEST_TRAJ = 89,
    TARGET_UNIT_TARGET_MINIPET = 90,
    TARGET_DEST_DEST_RADIUS = 91,
    TARGET_UNIT_SUMMONER = 92,
    TARGET_CORPSE_SRC_AREA_ENEMY = 93, // NYI
    TARGET_UNIT_VEHICLE = 94,
    TARGET_UNIT_TARGET_PASSENGER = 95,
    TARGET_UNIT_PASSENGER_0 = 96,
    TARGET_UNIT_PASSENGER_1 = 97,
    TARGET_UNIT_PASSENGER_2 = 98,
    TARGET_UNIT_PASSENGER_3 = 99,
    TARGET_UNIT_PASSENGER_4 = 100,
    TARGET_UNIT_PASSENGER_5 = 101,
    TARGET_UNIT_PASSENGER_6 = 102,
    TARGET_UNIT_PASSENGER_7 = 103,
    TARGET_UNIT_CONE_ENEMY_104 = 104,
    TARGET_UNIT_UNK_105 = 105, // 1 spell
    TARGET_DEST_CHANNEL_CASTER = 106,
    TARGET_UNK_DEST_AREA_UNK_107 = 107, // not enough info - only generic spells avalible
    TARGET_GAMEOBJECT_CONE = 108,
    TARGET_DEST_UNK_110 = 110, // 1 spell
    TARGET_UNK_111 = 111,
    TARGET_UNK_112 = 112,
    TARGET_UNK_113 = 113,
    TARGET_UNK_114 = 114,
    TARGET_UNK_115 = 115,
    TARGET_UNK_116 = 116,
    TARGET_UNK_117 = 117,
    TARGET_UNK_118 = 118,
    TARGET_UNK_119 = 119,
    TARGET_UNK_120 = 120,
    TARGET_UNK_121 = 121,
    TARGET_UNK_122 = 122,
    TARGET_UNK_123 = 123,
    TARGET_UNK_124 = 124,
    TARGET_UNK_125 = 125,
    TARGET_UNK_126 = 126,
    TARGET_UNK_127 = 127,
    TARGET_UNK_128 = 128,
    TARGET_UNK_129 = 129,
    TARGET_UNK_130 = 130,
    TARGET_UNK_131 = 131,
    TARGET_UNK_132 = 132,
    TARGET_UNK_133 = 133,
    TARGET_UNK_134 = 134,
    TARGET_UNK_135 = 135,
    TARGET_UNK_136 = 136,
    TARGET_UNK_137 = 137,
    TARGET_UNK_138 = 138,
    TARGET_UNK_139 = 139,
    TARGET_UNK_140 = 140,
    TARGET_UNK_141 = 141,
    TARGET_UNK_142 = 142,
    TARGET_UNK_143 = 143,
    TARGET_UNK_144 = 144,
    TARGET_UNK_145 = 145,
    TARGET_UNK_146 = 146,
    TARGET_UNK_147 = 147,
    TARGET_UNK_148 = 148,
    TOTAL_SPELL_TARGETS
};

public enum SpellDamageClass
{
    NONE = 0,
    MAGIC = 1,
    MELEE = 2,
    RANGED = 3
};

public enum SpellPreventionType
{
    NO_ACTIONS = 0,
    SILENCE = 1,
    PACIFY = 2,
};

public enum SpellInterruptFlags
{
    NONE = 0x00,
    MOVEMENT = 0x01,
    PUSH_BACK = 0x02,
    INTERRUPT = 0x08,
    ABORT_ON_DMG = 0x10,
};

public enum SpellFamilyNames
{
    GENERIC = 0,
    MAGE = 3,
    WARRIOR = 4,
    WARLOCK = 5,
    PRIEST = 6,
    DRUID = 7,
    ROGUE = 8,
    HUNTER = 9,
    PALADIN = 10,
    SHAMAN = 11,
    POTION = 13,
    DEATHKNIGHT = 15,
    PET = 17,
    MONK = 53,
    WARLOCK_PET = 57,
};

public enum SpellEffectType
{
    NONE = 0,
    INSTAKILL = 1,
    SCHOOL_DAMAGE = 2,
    DUMMY = 3,
    PORTAL_TELEPORT = 4, // Unused (4.3.4)
    TELEPORT_UNITS_OLD = 5, // Unused (7.0.3)
    APPLY_AURA = 6,
    ENVIRONMENTAL_DAMAGE = 7,
    POWER_DRAIN = 8,
    HEALTH_LEECH = 9,
    HEAL = 10,
    BIND = 11,
    PORTAL = 12,
    RITUAL_BASE = 13, // Unused (4.3.4)
    INCREASE_CURRENCY_CAP = 14,
    RITUAL_ACTIVATE_PORTAL = 15, // Unused (4.3.4)
    QUEST_COMPLETE = 16,
    WEAPON_DAMAGE_NOSCHOOL = 17,
    RESURRECT = 18,
    ADD_EXTRA_ATTACKS = 19,
    DODGE = 20,
    EVADE = 21,
    PARRY = 22,
    BLOCK = 23,
    CREATE_ITEM = 24,
    WEAPON = 25,
    DEFENSE = 26,
    PERSISTENT_AREA_AURA = 27,
    SUMMON = 28,
    LEAP = 29,
    ENERGIZE = 30,
    WEAPON_PERCENT_DAMAGE = 31,
    TRIGGER_MISSILE = 32,
    OPEN_LOCK = 33,
    SUMMON_CHANGE_ITEM = 34,
    APPLY_AREA_AURA_PARTY = 35,
    LEARN_SPELL = 36,
    SPELL_DEFENSE = 37,
    DISPEL = 38,
    LANGUAGE = 39,
    DUAL_WIELD = 40,
    JUMP = 41,
    JUMP_DEST = 42,
    TELEPORT_UNITS_FACE_CASTER = 43,
    SKILL_STEP = 44,
    PLAY_MOVIE = 45,
    SPAWN = 46,
    TRADE_SKILL = 47,
    STEALTH = 48,
    DETECT = 49,
    TRANS_DOOR = 50,
    FORCE_CRITICAL_HIT = 51, // Unused (4.3.4)
    SET_MAX_BATTLE_PET_COUNT = 52,
    ENCHANT_ITEM = 53,
    ENCHANT_ITEM_TEMPORARY = 54,
    TAMECREATURE = 55,
    SUMMON_PET = 56,
    LEARN_PET_SPELL = 57,
    WEAPON_DAMAGE = 58,
    CREATE_RANDOM_ITEM = 59,
    PROFICIENCY = 60,
    SEND_EVENT = 61,
    POWER_BURN = 62,
    THREAT = 63,
    TRIGGER_SPELL = 64,
    APPLY_AREA_AURA_RAID = 65,
    CREATE_MANA_GEM = 66,
    HEAL_MAX_HEALTH = 67,
    INTERRUPT_CAST = 68,
    DISTRACT = 69,
    PULL = 70,
    PICKPOCKET = 71,
    ADD_FARSIGHT = 72,
    UNTRAIN_TALENTS = 73,
    APPLY_GLYPH = 74,
    HEAL_MECHANICAL = 75,
    SUMMON_OBJECT_WILD = 76,
    SCRIPT_EFFECT = 77,
    ATTACK = 78,
    SANCTUARY = 79,
    ADD_COMBO_POINTS = 80,
    PUSH_ABILITY_TO_ACTION_BAR = 81,
    BIND_SIGHT = 82,
    DUEL = 83,
    STUCK = 84,
    SUMMON_PLAYER = 85,
    ACTIVATE_OBJECT = 86,
    GAMEOBJECT_DAMAGE = 87,
    GAMEOBJECT_REPAIR = 88,
    GAMEOBJECT_SET_DESTRUCTION_STATE = 89,
    KILL_CREDIT = 90,
    THREAT_ALL = 91,
    ENCHANT_HELD_ITEM = 92,
    FORCE_DESELECT = 93,
    SELF_RESURRECT = 94,
    SKINNING = 95,
    CHARGE = 96,
    CAST_BUTTON = 97,
    KNOCK_BACK = 98,
    DISENCHANT = 99,
    INEBRIATE = 100,
    FEED_PET = 101,
    DISMISS_PET = 102,
    REPUTATION = 103,
    SUMMON_OBJECT_SLOT1 = 104,
    SURVEY = 105,
    CHANGE_RAID_MARKER = 106,
    SHOW_CORPSE_LOOT = 107,
    DISPEL_MECHANIC = 108,
    RESURRECT_PET = 109,
    DESTROY_ALL_TOTEMS = 110,
    DURABILITY_DAMAGE = 111,
    ATTACK_ME = 114,
    DURABILITY_DAMAGE_PCT = 115,
    SKIN_PLAYER_CORPSE = 116,
    SPIRIT_HEAL = 117,
    SKILL = 118,
    APPLY_AREA_AURA_PET = 119,
    TELEPORT_GRAVEYARD = 120,
    NORMALIZED_WEAPON_DMG = 121,
    SEND_TAXI = 123,
    PULL_TOWARDS = 124,
    MODIFY_THREAT_PERCENT = 125,
    STEAL_BENEFICIAL_BUFF = 126,
    PROSPECTING = 127,
    APPLY_AREA_AURA_FRIEND = 128,
    APPLY_AREA_AURA_ENEMY = 129,
    REDIRECT_THREAT = 130,
    PLAY_SOUND = 131,
    PLAY_MUSIC = 132,
    UNLEARN_SPECIALIZATION = 133,
    KILL_CREDIT2 = 134,
    CALL_PET = 135,
    HEAL_PCT = 136,
    ENERGIZE_PCT = 137,
    LEAP_BACK = 138,
    CLEAR_QUEST = 139,
    FORCE_CAST = 140,
    FORCE_CAST_WITH_VALUE = 141,
    TRIGGER_SPELL_WITH_VALUE = 142,
    APPLY_AREA_AURA_OWNER = 143,
    KNOCK_BACK_DEST = 144,
    PULL_TOWARDS_DEST = 145,
    ACTIVATE_RUNE = 146,
    QUEST_FAIL = 147,
    TRIGGER_MISSILE_SPELL_WITH_VALUE = 148,
    CHARGE_DEST = 149,
    QUEST_START = 150,
    TRIGGER_SPELL_2 = 151,
    SUMMON_RAF_FRIEND = 152,
    CREATE_TAMED_PET = 153,
    DISCOVER_TAXI = 154,
    TITAN_GRIP = 155,
    ENCHANT_ITEM_PRISMATIC = 156,
    CREATE_ITEM_2 = 157,
    MILLING = 158,
    ALLOW_RENAME_PET = 159,
    FORCE_CAST_2 = 160,
    TALENT_SPEC_COUNT = 161,
    TALENT_SPEC_SELECT = 162,
    OBLITERATE_ITEM = 163,
    REMOVE_AURA = 164,
    DAMAGE_FROM_MAX_HEALTH_PCT = 165,
    GIVE_CURRENCY = 166,
    UPDATE_PLAYER_PHASE = 167,
    ALLOW_CONTROL_PET = 168, // NYI
    DESTROY_ITEM = 169,
    UPDATE_ZONE_AURAS_AND_PHASES = 170, // NYI
    RESURRECT_WITH_AURA = 172,
    UNLOCK_GUILD_VAULT_TAB = 173, // Guild tab unlocked (guild perk)
    APPLY_AURA_ON_PET = 174, // NYI
    SANCTUARY_2 = 176, // NYI
    CREATE_AREATRIGGER = 179,
    UPDATE_AREATRIGGER = 180, // NYI
    REMOVE_TALENT = 181,
    DESPAWN_AREATRIGGER = 182,
    REPUTATION_2 = 184, // NYI
    RANDOMIZE_ARCHAEOLOGY_DIGSITES = 187, // NYI
    LOOT = 189, // NYI, lootid in MiscValue ?
    TELEPORT_TO_DIGSITE = 191, // NYI
    UNCAGE_BATTLEPET = 192,
    START_PET_BATTLE = 193,
    PLAY_SCENE = 198, // NYI
    HEAL_BATTLEPET_PCT = 200, // NYI
    ENABLE_BATTLE_PETS = 201, // NYI
    CHANGE_BATTLEPET_QUALITY = 204,
    LAUNCH_QUEST_CHOICE = 205,
    ALTER_ITEM = 206, // NYI
    LAUNCH_QUEST_TASK = 207, // Starts one of the "progress bar" quests
    LEARN_GARRISON_BUILDING = 210,
    LEARN_GARRISON_SPECIALIZATION = 211,
    CREATE_GARRISON = 214,
    UPGRADE_CHARACTER_SPELLS = 215, // Unlocks boosted players' spells (ChrUpgrade*.db2)
    CREATE_SHIPMENT = 216,
    UPGRADE_GARRISON = 217,
    ADD_GARRISON_FOLLOWER = 220,
    CREATE_HEIRLOOM_ITEM = 222,
    CHANGE_ITEM_BONUSES = 223,
    ACTIVATE_GARRISON_BUILDING = 224,
    GRANT_BATTLEPET_LEVEL = 225,
    TELEPORT_TO_LFG_DUNGEON = 227,
    SET_FOLLOWER_QUALITY = 229,
    INCREASE_FOLLOWER_ITEM_LEVEL = 230,
    INCREASE_FOLLOWER_EXPERIENCE = 231,
    REMOVE_PHASE = 232,
    RANDOMIZE_FOLLOWER_ABILITIES = 233,
    GIVE_EXPERIENCE = 236, // Increases players XP
    GIVE_RESTED_EXPERIENCE_BONUS = 237,
    INCREASE_SKILL = 238,
    END_GARRISON_BUILDING_CONSTRUCTION = 239, // Instantly finishes building construction
    GIVE_ARTIFACT_POWER = 240,
    GIVE_ARTIFACT_POWER_NO_BONUS = 242, // Unaffected by Artifact Knowledge
    APPLY_ENCHANT_ILLUSION = 243,
    LEARN_FOLLOWER_ABILITY = 244,
    UPGRADE_HEIRLOOM = 245,
    FINISH_GARRISON_MISSION = 246,
    ADD_GARRISON_MISSION = 247,
    FINISH_SHIPMENT = 248,
    FORCE_EQUIP_ITEM = 249,
    TAKE_SCREENSHOT = 250, // Serverside marker for selfie screenshot - achievement check
    SET_GARRISON_CACHE_SIZE = 251,
    TELEPORT_UNITS = 252,
    GIVE_HONOR = 253,
    LEARN_TRANSMOG_SET = 255,
    TOTAL_SPELL_EFFECTS = 256,
};

public enum DiminishingLevels
{
    LEVEL_1 = 0,
    LEVEL_2 = 1,
    LEVEL_3 = 2,
    LEVEL_4 = 3,
    IMMUNE = 3,
    TAUNT_IMMUNE = 4
};

public enum DiminishingGroup
{
    NONE = 0,
    ROOT = 1,
    STUN = 2,
    INCAPACITATE = 3,
    DISORIENT = 4,
    SILENCE = 5,
    AOE_KNOCKBACK = 6,
    TAUNT = 7,
    LIMITONLY = 8,
};

public enum SpellMissInfo
{
    NONE = 0,
    MISS = 1,
    RESIST = 2,
    DODGE = 3,
    PARRY = 4,
    BLOCK = 5,
    EVADE = 6,
    IMMUNE = 7,
    DEFLECT = 9,
    ABSORB = 10,
    REFLECT = 11
};

public class SpellDamage
{
    public Unit Target;
    public Unit Attacker;
    public Guid CastId;
    public int SpellID;
    public int Damage;
    public int Absorb;
    public int Resist;
    public bool Crit;
    public bool PeriodicLog;

    public SpellSchoolMask SchoolMask;
    public HitInfo HitInfo;

    public SpellDamage(Unit attacker, Unit target, int _SpellID, SpellSchoolMask _schoolMask, Guid _castId = default(Guid))
    {
        Attacker = attacker;
        Target = target;
        CastId = _castId;
        SpellID = _SpellID;

        SchoolMask = _schoolMask;

        Damage = 0;
        Absorb = 0;
        Resist = 0;
        Crit = false;
        PeriodicLog = false;

        HitInfo = HitInfo.AffectsVictim;
    }
};

public struct CleanDamage
{
    public int AbsorbedDamage;
    public int MitigatedDamage;

    public CleanDamage(int absorb, int mitigated)
    {
        AbsorbedDamage = absorb;
        MitigatedDamage = mitigated;
    }
};

public enum DamageEffectType
{
    DIRECT_DAMAGE = 0,                            // used for normal weapon damage (not for class abilities or spells)
    SPELL_DIRECT_DAMAGE = 1,                            // spell/class abilities damage
    DOT = 2,
    HEAL = 3,
    NODAMAGE = 4,                            // used also in case when damage applied to health but not applied to spell channelInterruptFlags/etc
    SELF_DAMAGE = 5
};