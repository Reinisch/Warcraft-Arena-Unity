using System;

namespace Core
{
    public enum MapTypes
    {
        Common = 0,                             // none
        Instance = 1,                           // party
        Raid = 2,                               // raid
        Battleground = 3,                       // pvp
        Arena = 4,                              // arena
        Scenario = 5                            // scenario
    }

    public enum EnterState
    {
        CanEnter = 0,
        AlreadyInMap = 1, // Player is already in the map
        NoEntry, // No map entry was found for the target map ID
        UninstancedDungeon, // No instance template was found for dungeon map
        DifficultyUnavailable, // Requested instance difficulty is not available for target map
        NotInRaid, // Target instance is a raid instance and the player is not in a raid group
        CorpseInDifferentInstance, // Player is dead and their corpse is not in target instance
        InstanceBindMismatch, // Player's permanent instance save is not compatible with their group's current instance bind
        TooManyInstances, // Player has entered too many instances recently
        MaxPlayers, // Target map already has the maximum number of players allowed
        ZoneInCombat, // A boss encounter is currently in progress on the target map
        UnspecifiedReason
    }

    [Flags]
    public enum MapFlags
    {
        CanToggleDifficulty = 0x0100,
        FlexLocking = 0x8000,           // All difficulties share completed encounters lock, not bound to a single instance id
        // heroic difficulty flag overrides it and uses instance id bind
        Garrison = 0x4000000
    }

    [Flags]
    public enum MapAreaHeaderFlags
    {
        NoArea = 0x0001
    }

    [Flags]
    public enum MapHeightFlags
    {
        NoHeight = 0x0001,
        AsShort = 0x0002,
        AsByte = 0x0004,
        HasFlightBounds = 0x0008
    }

    [Flags]
    public enum LiquidHeaderFlags
    {
        NoType = 0x0001,
        NoHeight = 0x0002
    }

    [Flags]
    public enum LiquidHeightStatus
    {
        NoWater = 0x00000000,
        AboveWater = 0x00000001,
        WaterWalk = 0x00000002,
        InWater = 0x00000004,
        UnderWater = 0x00000008
    }

    [Flags]
    public enum MapLiquidType
    {
        NoWater = 0x00,
        Water = 0x01,
        Ocean = 0x02,
        Magma = 0x04,
        Slime = 0x08,

        DarkWater = 0x10,
        WmoWater = 0x20,

        AllLiquids = Water | Ocean | Magma | Slime
    }

    public enum InstanceResetMethod
    {
        All,
        ChangeDifficulty,
        Global,
        GroupDisband,
        GroupJoin,
        RespawnDelay
    }
}