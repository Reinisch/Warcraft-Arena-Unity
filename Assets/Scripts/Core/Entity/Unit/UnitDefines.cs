using System;

namespace Core
{
    public enum UnitInfoOffsets : byte
    {
        Race = 0,
        Class = 1,
        Gender = 3
    }

    [Flags]
    public enum UnitTypeMask
    {
        None = 0,
        Summon = 1 << 0,
        Minion = 1 << 1,
        Guardian = 1 << 2,
        Totem = 1 << 3,
        Pet = 1 << 4,
        Vehicle = 1 << 5,
        Puppet = 1 << 6,
        HunterPet = 1 << 7,
        ControlableGuardian = 1 << 8,
        Accessory = 1 << 9,
    }

    [Flags]
    public enum FactionTemplateFlags : short
    {
        EnemySpar = 1 << 0,             // guessed, sparring with enemies?
        Pvp = 1 << 1,                   // flagged for PvP
        ContestedGuard = 1 << 2,        // faction will attack players that were involved in PvP combats
        HostileByDefault = 1 << 3
    }

    [Flags]
    public enum FactionMasks : short
    {                               // if none flags set then non-aggressive creature
        Player = 1 << 0,            // any player
        Alliance = 1 << 1,          // player or creature from alliance team
        Horde = 1 << 2,             // player or creature from horde team
        Monster = 1 << 3            // aggressive creature from monster team
    }

    public enum ReputationRank
    {
        Hated = 0,
        Hostile = 1,
        Unfriendly = 2,
        Neutral = 3,
        Friendly = 4,
        Honored = 5,
        Revered = 6,
        Exalted = 7
    }

    public enum MeleeHitOutcome
    {
        Evade,
        Miss,
        Dodge,
        Block,
        Parry,
        Glancing,
        Crit,
        Crushing,
        Normal
    }

    public enum CharmType
    {
        Charm,
        Possess,
        Vehicle,
        Convert
    }

    // high byte (3 from 0..3) of EntityFields.BaseFlags
    public enum ShapeshiftForm : byte
    {
        None = 0,
        CatForm = 1,
        TreeOfLife = 2,
        TravelForm = 3,
        AquaticForm = 4,
        BearForm = 5,
        Ambient = 6,
        Ghoul = 7,
        DireBearForm = 8,
        CraneStance = 9,
        TharonjaSkeleton = 10,
        DarkmoonTestOfStrength = 11,
        BlbPlayer = 12,
        ShadowDance = 13,
        CreatureBear = 14,
        CreatureCat = 15,
        GhostWolf = 16,
        BattleStance = 17,
        DefensiveStance = 18,
        BerserkerStance = 19,
        SerpentStance = 20,
        Zombie = 21,
        Metamorphosis = 22,
        OxStance = 23,
        TigerStance = 24,
        Undead = 25,
        Frenzy = 26,
        FlightEpic = 27,
        Shadowform = 28,
        FlightForm = 29,
        Stealth = 30,
        MoonkinForm = 31,
        SpiritOfRedemption = 32,
        GladiatorStance = 33
    }

    // low byte (0 from 0..3) of EntityFields.BaseFlags
    public enum SheathState : byte
    {
        Unarmed = 0,                            // non prepared weapon
        Melee = 1,                              // prepared melee weapon
        Ranged = 2                              // prepared ranged weapon
    }

    // byte (1 from 0..3) of EntityFields.BaseFlags
    public enum UnitPvpStateFlags : byte
    {
        Pvp = 0x01,
        FfaPvp = 0x04,
        Sanctuary = 0x08
    }

    public enum WeaponDamageRange
    {
        MinDamage,
        MaxDamage
    }

    public enum PlayerTotemType
    {
        TotemFire = 63,
        TotemEarth = 81,
        TotemWater = 82,
        TotemAir = 83
    }

    [Flags]
    public enum UnitEventType
    {
        /// <summary> Player/Pet changed on/offline status. </summary>
        RefOnlineStatus = 1 << 0,
        /// <summary> Threat for Player/Pet changed. </summary>
        RefThreatChange = 1 << 1,
        /// <summary> Player/Pet will be removed from list (dead). [for internal use] </summary>
        RefRemoveFromList = 1 << 2,
        /// <summary> Player/Pet entered/left  water or some other place where it is/was not accessible for the creature. </summary>
        RefAsseccibleStatus = 1 << 3,
        /// <summary> Threat list is going to be sorted (if dirty flag is set). </summary>
        SortList = 1 << 4,
        /// <summary> New target should be fetched, could tbe the current target as well. </summary>
        SetNextTarget = 1 << 5,
        /// <summary> A new victim (target) was set. Could be NULL. </summary>
        VictimChanged = 1 << 6,

        RefEventMask = RefOnlineStatus | RefThreatChange | RefRemoveFromList | RefAsseccibleStatus,
        ManagerEventMask = SortList | SetNextTarget | VictimChanged,
        AllEventMask = 0x7fffffff,
    }

    public enum VictimState
    {
        Intact,
        Hi,
        Dodge,
        Parry,
        Interrupt,
        Evades,
        Immune,
        Deflects
    }

    public enum HitInfo
    {
        Normalswing = 0x00000000,
        AffectsVictim = 0x00000001,
        Offhand = 0x00000002,
        Miss = 0x00000004,
        FullAbsorb = 0x00000008,
        PartialAbsorb = 0x00000010,
        CriticalHit = 0x00000020,
        RageGain = 0x00000040,
    }

    public enum DeathState
    {
        Alive = 0,
        JustDied = 1,
        Corpse = 2,
        Dead = 3,
        JustRespawned = 4
    }

    [Flags]
    public enum UnitState
    {
        None = 0x00000000,
        Died = 0x00000001,
        MeleeAttacking = 0x00000002,
        Stunned = 0x00000008,
        Roaming = 0x00000010,
        Chase = 0x00000020,
        Fleeing = 0x00000080,
        InFlight = 0x00000100,                     // player is in flight mode
        Follow = 0x00000200,
        Root = 0x00000400,
        Confused = 0x00000800,
        Distracted = 0x00001000,
        Isolated = 0x00002000,                  // area auras do not affect other players
        AttackPlayer = 0x00004000,
        Casting = 0x00008000,
        Possessed = 0x00010000,
        Charging = 0x00020000,
        Jumping = 0x00040000,
        Move = 0x00100000,
        Rotating = 0x00200000,
        Evade = 0x00400000,
        RoamingMove = 0x00800000,
        ConfusedMove = 0x01000000,
        FleeingMove = 0x02000000,
        ChaseMove = 0x04000000,
        FollowMove = 0x08000000,
        IgnorePathfinding = 0x10000000,

        Unattackable = InFlight,
        Moving = RoamingMove | ConfusedMove | FleeingMove | ChaseMove | FollowMove,
        Controlled = Confused | Stunned | Fleeing,
        LostControl = Controlled | Jumping | Charging,
        Sightless = LostControl | Evade,
        CannotAutoattack = LostControl | Casting,
        CannotTurn = LostControl | Rotating,
        CantMove = Root | Stunned | Died | Distracted
    }

    public enum CombatRating
    {
        Dodge,
        Crit,
        MultiStrike,
        Speed,
        Haste,
        ArmorPenetration,
        Mastery,
        PvpPower,
        ResilenceCritTaken,
        ResilencePlayerDamage,
    }

    public enum UnitMoveType
    {
        Run,
        RunBack,
        TurnRate,
        PitchRate,
    }

    [Flags]
    public enum ActiveStates
    {
        Passive = 0x01,                 // 0x01 - passive
        Disabled = 0x81,                // 0x80 - castable
        Enabled = 0xC1,                 // 0x40 | 0x80 - auto cast + castable
        Command = 0x07,                 // 0x01 | 0x02 | 0x04
        Reaction = 0x06,                // 0x02 | 0x04
        Decide = 0x00                   // custom
    }

    public enum ReactStates
    {
        Passive = 0,
        Defensive = 1,
        Aggressive = 2,
        Assist = 3
    }

    public enum CommandStates : byte
    {
        Stay = 0,
        Follow = 1,
        Attack = 2,
        Abandon = 3,
        MoveTo = 4
    }

    public enum ActionBarIndex
    {
        Start = 0,
        PetSpellStart = 3,
        PetSpellEnd = 7,
        End = 10
    }
}
