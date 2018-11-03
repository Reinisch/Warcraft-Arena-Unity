using System;

namespace Core
{
    [Flags]
    public enum EntityTypeMask
    {
        Object = 0x0001,
        Item = 0x0002,
        Container = 0x0004,
        Unit = 0x0008,
        Player = 0x0010,
        GameObject = 0x0020,
        DynamicObject = 0x0040,
        Corpse = 0x0080,
        Areatrigger = 0x0100,
        Sceneobject = 0x0200,
        Conversation = 0x0400,
        Seer = Player | Unit | DynamicObject
    }

    [Flags]
    public enum UpdateFlags
    {
        None = 0x0000,
        Self = 0x0001,
        Transport = 0x0002,
        HasTarget = 0x0004,
        Living = 0x0008,
        StationaryPosition = 0x0010,
        Vehicle = 0x0020,
        TransportPosition = 0x0040,
        Rotation = 0x0080,
        Animkits = 0x0100,
    }

    [Flags]
    public enum UnitFlags
    {
        ServerControlled = 1 << 0,
        NonAttackable = 1 << 1,
        DisableMove = 1 << 2,
        PvpAttackable = 1 << 3,
        Preparation = 1 << 4,
        Looting = 1 << 5,
        Pvp = 1 << 6,
        Silenced = 1 << 7,
        Pacified = 1 << 8,
        Stunned = 1 << 9,
        InCombat = 1 << 10,
        Disarmed = 1 << 11,
        Confused = 1 << 12,
        Fleeing = 1 << 13,
        PlayerControlled = 1 << 14,
        NotSelectable = 1 << 15,
        Skinnable = 1 << 16,
        Mount = 1 << 17,
        Sheathe = 1 << 18,
        FeignDeath = 1 << 19,
        MirrorImage = 1 << 20,
        ForceMovement = 1 << 21,
        DisarmOffhand = 1 << 22,
        RegeneratePower = 1 << 23,
        DisableTurn = 1 << 24,
        AllowCheatSpells = 1 << 25,
        NoActions = 1 << 26,
    }

    [Flags]
    public enum NpcFlags : long
    {
        None = 1 << 0,
        Gossip = 1 << 1,
        Questgiver = 1 << 2,
        Trainer = 1 << 5,
        TrainerClass = 1 << 6,
        TrainerProfession = 1 << 7,
        Vendor = 1 << 8,
        VendorAmmo = 1 << 9,
        VendorFood = 1 << 10,
        VendorPoison = 1 << 11,
        VendorReagent = 1 << 12,
        Repair = 1 << 13,
        FlightMaster = 1 << 14,
        SpiritHealer = 1 << 15,
        SpiritGuide = 1 << 16,
        Innkeeper = 1 << 17,
        Banker = 1 << 18,
        Petitioner = 1 << 19,
        TabardDesigner = 1 << 20,
        BattleMaster = 1 << 21,
        Auctioneer = 1 << 22,
        StableMaster = 1 << 23,
        GuildBanker = 1 << 24,
        Spellclick = 1 << 25,
        PlayerVehicle = 1 << 26,
        Mailbox = 1 << 27,
        ArtifactPowerRespec = 1 << 28,
        Transmogrifier = 1 << 29,
        Vaultkeeper = 1 << 30,
        BlackMarket = 1 << 31,
        ItemUpgradeMaster = 1 << 32,
        GarrisonArchitect = 1 << 33,
        ShipmentCrafter = 1 << 34,
        GarrisonMissionNpc = 1 << 35,
        TradeskillNpc = 1 << 36,
        BlackMarketView = 1 << 37,
    }

    public enum EntityType
    {
        Entity = 0,
        Item = 1,
        Container = 2,
        Unit = 3,
        Player = 4,
        GameEntity = 5,
        DynamicEntity = 6,
        Corpse = 7,
        AreaTrigger = 8,
        SceneObject = 9,
        Creature = 10,
    }

    [Flags]
    public enum EnityTypeMask
    {
        Entity = 1 << EntityType.Entity,
        Item = 1 << EntityType.Item,
        Container = 1 << EntityType.Container,
        Unit = 1 << EntityType.Unit,
        Player = 1 << EntityType.Player,
        GameEntity = 1 << EntityType.GameEntity,
        DynamicEntity = 1 << EntityType.DynamicEntity,
        AreaTrigger = 1 << EntityType.AreaTrigger,
        SceneObject = 1 << EntityType.SceneObject,
    }

    public enum GameEntityTypes : byte
    {
        Door = 0,
        Button = 1,
        QuestGiver = 2,
        Chest = 3,
        Binder = 4,
        Generic = 5,
        Trap = 6,
        Chair = 7,
        SpellFocus = 8,
        Text = 9,
        Goober = 10,
        Transport = 11,
        AreaDamage = 12,
        Camera = 13,
        MapObject = 14,
        MapObjTransport = 15,
        DuelArbiter = 16,
        FishingNode = 17,
        Ritual = 18,
        Mailbox = 19,
        Guardpost = 21,
        Spellcaster = 22,
        MeetingStone = 23,
        Flagstand = 24,
        FishingHole = 25,
        Flagdrop = 26,
        MiniGame = 27,
        ControlZone = 29,
        AuraGenerator = 30,
        DungeonDifficulty = 31,
        BarberChair = 32,
        DestructibleBuilding = 33,
        GuildBank = 34,
        Trapdoor = 35,
        NewFlag = 36,
        NewFlagDrop = 37,
        GarrisonBuilding = 38,
        GarrisonPlot = 39,
        ClientCreature = 40,
        ClientItem = 41,
        CapturePoint = 42,
        PhaseableMo = 43,
        GarrisonMonument = 44,
        GarrisonShipment = 45,
        GarrisonMonumentPlaque = 46,
        ArtifactForge = 47,
        UiLink = 48,
        KeystoneReceptacle = 49,
        GatheringNode = 50,
        ChallengeModeReward = 51
    }

    public enum ArenaTeam
    {
        Red,
        Blue,
    }
}