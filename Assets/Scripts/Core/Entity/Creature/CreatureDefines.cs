using System;

namespace Core
{
    public enum CreatureType
    {
        Beast = 1,
        Dragonkin = 2,
        Demon = 3,
        Elemental = 4,
        Giant = 5,
        Undead = 6,
        Humanoid = 7,
        Critter = 8,
        Mechanical = 9,
        NotSpecified = 10,
        Totem = 11,
        NonCombatPet = 12,
        GasCloud = 13,
        WildPet = 14,
        Aberration = 15,
    }

    public enum CreatureFamily
    {
        Wolf = 1,
        Cat = 2,
        Spider = 3,
        Bear = 4,
        Boar = 5,
        Crocolisk = 6,
        CarrionBird = 7,
        Crab = 8,
        Gorilla = 9,
        HorseCustom = 10,
        Raptor = 11,
        Tallstrider = 12,
        Felhunter = 15,
        Voidwalker = 16,
        Succubus = 17,
        Doomguard = 19,
        Scorpid = 20,
        Turtle = 21,
        Imp = 23,
        Bat = 24,
        Hyena = 25,
        BirdOfPrey = 26,
        WindSerpent = 27,
        RemoteControl = 28,
        Felguard = 29,
        Dragonhawk = 30,
        Ravager = 31,
        WarpStalker = 32,
        Sporebat = 33,
        NetherRay = 34,
        Serpent = 35,
        Moth = 37,
        Chimaera = 38,
        Devilsaur = 39,
        Ghoul = 40,
        Silithid = 41,
        Worm = 42,
        Rhino = 43,
        Wasp = 44,
        CoreHound = 45,
        SpiritBeast = 46,
        WaterElemental = 49,
        Fox = 50,
        Monkey = 51,
        Dog = 52,
        Beetle = 53,
        ShaleSpider = 55,
        Zombie = 56,
        BeetleOld = 57,
        Silithid2 = 59,
        Wasp2 = 66,
        Hydra = 68,
        Felimp = 100,
        Voidlord = 101,
        Shivara = 102,
        Observer = 103,
        Wrathguard = 104,
        Infernal = 108,
        Fireelemental = 116,
        Earthelemental = 117,
        Crane = 125,
        Waterstrider = 126,
        Porcupine = 127,
        Quilen = 128,
        Goat = 129,
        Basilisk = 130,
        Direhorn = 138,
        Stormelemental = 145,
        Mtwaterelemental = 146,
        Torrorguard = 147,
        Abyssal = 148,
        Rylak = 149,
        Riverbeast = 150,
        Stag = 151
    }

    public enum CreatureEliteType
    {
        Normal = 0,
        Elite = 1,
        Rareelite = 2,
        Worldboss = 3,
        Rare = 4,
        Weak = 6
    }

    public enum TempSummonType
    {
        TimedOrDeadDespawn = 1,             // despawns after a specified time OR when the creature disappears
        TimedOrCorpseDespawn = 2,           // despawns after a specified time OR when the creature dies
        TimedDespawn = 3,                   // despawns after a specified time
        TimedDespawnOutOfCombat = 4,        // despawns after a specified time after the creature is out of combat
        CorpseDespawn = 5,                  // despawns instantly after death
        CorpseTimedDespawn = 6,             // despawns after a specified time after death
        DeadDespawn = 7,                    // despawns when the creature disappears
        ManualDespawn = 8                   // despawns when UnSummon() is called
    }

    public enum TrainerType
    {
        Class = 0,
        Mounts = 1,
        Tradeskills = 2,
        Pets = 3
    }

    public enum ChatType
    {
        Say = 0,
        Yell = 1,
        TextEmote = 2,
        BossEmote = 3,
        Whisper = 4,
        BossWhisper = 5,
        ZoneYell = 6,
    }

    [Flags]
    public enum CreatureDifficultyFlags : long
    {
        NoExperience = 1 << 0,
        NoLoot = 1 << 1,
        Unkillable = 1 << 2,
        Tameable = 1 << 3, // CREATURE_TYPEFLAGS_TAMEABLE
        ImmuneToPc = 1 << 4, // UNIT_FLAG_IMMUNE_TO_PC
        ImmuneToNpc = 1 << 5, // UNIT_FLAG_IMMUNE_TO_NPC
        Sessile = 1 << 6, // Creature is rooted
        NotSelectable = 1 << 7, // UNIT_FLAG_NOT_SELECTABLE
        NoCorpseUponDeath = 1 << 8, // Creature instantly disappear when killed
        Boss = 1 << 9, // CREATURE_TYPEFLAGS_BOSS
        WaterBound = 1 << 10,
        CanPenetrateWater = 1 << 11,
        Ghost = 1 << 12, // CREATURE_TYPEFLAGS_GHOST
        DoNotPlayWoundParryAnimation = 1 << 13, // CREATURE_TYPEFLAGS_DO_NOT_PLAY_WOUND_PARRY_ANIMATION
        HideFactionTooltip = 1 << 14, // CREATURE_TYPEFLAGS_HIDE_FACTION_TOOLTIP
        IgnoreCombat = 1 << 15,
        SummonGuardIfInAggroRange = 1 << 16, // Creature will summon a guard if player is within its aggro range (even if creature doesn't attack per se)
        OnlySwim = 1 << 16, // UNIT_FLAG_UNK_15
        TflagUnk5 = 1 << 17, // CREATURE_TYPEFLAGS_UNK5
        LargeAoi = 1 << 18,  // UnitFlags2 0x200000
        ForcePartyMembersIntoCombat = 1 << 19,
        SpellAttackable = 1 << 20, // CREATURE_TYPEFLAGS_SPELL_ATTACKABLE
        DeadInteract = 1 << 21, // CREATURE_TYPEFLAGS_DEAD_INTERACT
        Herbloot = 1 << 22, // CREATURE_TYPEFLAGS_HERBLOOT
        Miningloot = 1 << 23, // CREATURE_TYPEFLAGS_MININGLOOT
        DontLogDeath = 1 << 24, // CREATURE_TYPEFLAGS_DONT_LOG_DEATH
        MountedCombat = 1 << 25, // CREATURE_TYPEFLAGS_MOUNTED_COMBAT
        HideBody = 1 << 26, // UNIT_FLAG2_UNK1
        ServerOnly = 1 << 27,
        CanSafeFall = 1 << 28,
        CanAssist = 1 << 29, // CREATURE_TYPEFLAGS_CAN_ASSIST
        KeepHealthPointsAtReset = 1 << 30,
        IsPetBarUsed = 1 << 31,  // CREATURE_TYPEFLAGS_IS_PET_BAR_USED
        InstantlyAppearModel = 1 << 32, // UNIT_FLAG2_INSTANTLY_APPEAR_MODEL
        MaskUid = 1 << 33, // CREATURE_TYPEFLAG_MASK_UID
        Engineerloot = 1 << 34, // CREATURE_TYPEFLAGS_ENGINEERLOOT
        CannotSwim = 1 << 35, // UNIT_FLAG_UNK_14
        Exotic = 1 << 36, // CREATURE_TYPEFLAGS_EXOTIC
        GiganticAoi = 1 << 37, // Since MoP, creatures with that flag have UnitFlags2 0x400000
        InfiniteAoi = 1 << 38, // Since MoP, creatures with that flag have UnitFlags2 0x40000000
        Waterwalking = 1 << 39,
        HideNameplate = 1 << 40, // CREATURE_TYPEFLAGS_HIDE_NAMEPLATE
        UseDefaultCollisionBox = 1 << 41, // CREATURE_TYPEFLAGS_USE_DEFAULT_COLLISION_BOX
        IsSiegeWeapon = 1 << 42, // CREATURE_TYPEFLAGS_IS_SIEGE_WEAPON
        ProjectileCollision = 1 << 43, // CREATURE_TYPEFLAGS_PROJECTILE_COLLISION
        CanBeMultitapped = 1 << 44,
        DoNotPlayMountedAnimations = 1 << 45, // CREATURE_TYPEFLAGS_DO_NOT_PLAY_MOUNTED_ANIMATIONS
        DisableTurn = 1 << 46, // UNIT_FLAG2_DISABLE_TURN
        IsLinkAll = 1 << 47, // CREATURE_TYPEFLAGS_IS_LINK_ALL
        HasNoBirthAnimation = 1 << 48, // SMSG_UPDATE_OBJECT's "NoBirthAnim"
        InteractOnlyWithCreator = 1 << 49, // CREATURE_TYPEFLAGS_INTERACT_ONLY_WITH_CREATOR
        DoNotPlayUnitEventSounds = 1 << 50, // CREATURE_TYPEFLAGS_DO_NOT_PLAY_UNIT_EVENT_SOUNDS
        HasNoShadowBlob = 1 << 51, // CREATURE_TYPEFLAGS_HAS_NO_SHADOW_BLOB
        ForceGossip = 1 << 52, // CREATURE_TYPEFLAGS_FORCE_GOSSIP
        DoNotSheathe = 1 << 53, // CREATURE_TYPEFLAGS_DO_NOT_SHEATHE
        IgnoreSpellMinRangeRestrictions = 1 << 54, // UnitFlags2 0x8000000
        PreventSwim = 1 << 55, // UnitFlags2 0x1000000
        HideInCombatLog = 1 << 56, // UnitFlags2 0x2000000
        DoNotTargetOnInteraction = 1 << 57, // CREATURE_TYPEFLAGS_DO_NOT_TARGET_ON_INTERACTION
        DoNotRenderObjectName = 1 << 58, // CREATURE_TYPEFLAGS_DO_NOT_RENDER_OBJECT_NAME
        UnitIsQuestBoss = 1 << 59,  // CREATURE_TYPEFLAGS_UNIT_IS_QUEST_BOSS
        CannotSwitchTargets = 1 << 60, // UnitFlags2 0x4000000
        CanInteractEvenIfHostile = 1 << 61, // UNIT_FLAG2_ALLOW_ENEMY_INTERACT
    }

    [Flags]
    public enum CreatureFlagsExtra
    {
        InstanceBind = 0x00000001,              // creature kill bind instance with killer and killer's group
        Civilian = 0x00000002,                  // not aggro (ignore faction/reputation hostility)
        NoParry = 0x00000004,                   // creature can't parry
        NoParryHasten = 0x00000008,             // creature can't counter-attack at parry
        NoBlock = 0x00000010,                   // creature can't block
        NoCrush = 0x00000020,                   // creature can't do crush attacks
        NoXpAtKill = 0x00000040,                // creature kill not provide XP
        Trigger = 0x00000080,                   // trigger creature
        NoTaunt = 0x00000100,                   // creature is immune to taunt auras and effect attack me
        Worldevent = 0x00004000,                // custom flag for world event creatures (left room for merging)
        Guard = 0x00008000,                     // Creature is guard
        NoCrit = 0x00020000,                    // creature can't do critical strikes
        NoSkillgain = 0x00040000,               // creature won't increase weapon skills
        TauntDiminish = 0x00080000,             // Taunt is a subject to diminishing returns on this creautre
        AllDiminish = 0x00100000,               // creature is subject to all diminishing returns as player are
        NoPlayerDamageReq = 0x00200000,         // creature does not need to take player damage for kill credit
        DungeonBoss = 0x10000000,               // creature is a dungeon boss (SET DYNAMICALLY, DO NOT ADD IN DB)
        IgnorePathfinding = 0x20000000,         // creature ignore pathfinding
        ImmunityKnockback = 0x40000000,         // creature is immune to knockback effects

        Allowed = InstanceBind | Civilian | NoParry | NoParryHasten | NoBlock |  NoCrush |
                  NoXpAtKill | Trigger | NoTaunt | Worldevent | NoCrit |  NoSkillgain | TauntDiminish |
                  AllDiminish | Guard | IgnorePathfinding | NoPlayerDamageReq | ImmunityKnockback
    }

    [Flags]
    public enum InhabitTypeValues
    {
        Ground = 1,
        Water = 2,
        Air = 4,
        Anywhere = Ground | Water | Air
    }

    [Flags]
    public enum CreatureTypeFlags
    {
        Tameable = 0x00000001, // Tameable by any hunter
        Ghost = 0x00000002, // Creature are also visible for not alive player. Allow gossip interaction if npcflag allow?
        Boss = 0x00000004,
        DoNotPlayWoundParryAnimation = 0x00000008,
        HideFactionTooltip = 0x00000010,
        Unk5 = 0x00000020,
        SpellAttackable = 0x00000040,
        DeadInteract = 0x00000080, // Player can interact with the creature if its dead (not player dead)
        Herbloot = 0x00000100, // Can be looted by herbalist
        Miningloot = 0x00000200, // Can be looted by miner
        DontLogDeath = 0x00000400, // Death event will not show up in combat log
        MountedCombat = 0x00000800, // Creature can remain mounted when entering combat
        CanAssist = 0x00001000,
        IsPetBarUsed = 0x00002000,
        MaskUid = 0x00004000,
        Engineerloot = 0x00008000, // Can be looted by engineer
        Exotic = 0x00010000, // Can be tamed by hunter as exotic pet
        UseDefaultCollisionBox = 0x00020000,
        IsSiegeWeapon = 0x00040000,
        ProjectileCollision = 0x00080000, // Projectiles can collide with this creature - interacts with TARGET_DEST_TRAJ
        HideNameplate = 0x00100000,
        DoNotPlayMountedAnimations = 0x00200000,
        IsLinkAll = 0x00400000,
        InteractOnlyWithCreator = 0x00800000,
        DoNotPlayUnitEventSounds = 0x01000000,
        HasNoShadowBlob = 0x02000000,
        TreatAsRaidUnit = 0x04000000, //! Creature can be targeted by spells that require target to be in caster's party/raid
        ForceGossip = 0x08000000,
        DoNotSheathe = 0x10000000,
        DoNotTargetOnInteraction = 0x20000000,
        DoNotRenderObjectName = 0x40000000,
    }
}