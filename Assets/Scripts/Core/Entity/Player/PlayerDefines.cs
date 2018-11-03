using System;

namespace Core
{
    public enum SkillFieldOffset
    {
        SkillIDOffset = 0,
        SkillStepOffset = 64,
        SkillRankOffset = SkillStepOffset + 64,
        SubskillStartRankOffset = SkillRankOffset + 64,
        SkillMaxRankOffset = SubskillStartRankOffset + 64,
        SkillTempBonusOffset = SkillMaxRankOffset + 64,
        SkillPermBonusOffset = SkillTempBonusOffset + 64
    }

    [Flags]
    public enum PlayerUnderwaterState
    {
        None = 0x00,
        Inwater = 0x01,             // terrain type is water and player is afflicted by it
        Inlava = 0x02,             // terrain type is lava and player is afflicted by it
        Inslime = 0x04,             // terrain type is lava and player is afflicted by it
        Indarkwater = 0x08,             // terrain type is dark water and player is afflicted by it

        ExistTimers = 0x10
    }

    public enum BuyBankSlotResult
    {
        TooMany = 0,
        InsufficientFunds = 1,
        Notbanker = 2,
        Ok = 3
    }

    public enum PlayerSpellState : byte
    {
        Unchanged = 0,
        Changed = 1,
        New = 2,
        Removed = 3,
        Temporary = 4
    }

    public enum TalentSpecialization
    {
        MageArcane = 62,
        MageFire = 63,
        MageFrost = 64,
        PaladinHoly = 65,
        PaladinProtection = 66,
        PaladinRetribution = 70,
        WarriorArms = 71,
        WarriorFury = 72,
        WarriorProtection = 73,
        DruidBalance = 102,
        DruidCat = 103,
        DruidBear = 104,
        DruidRestoration = 105,
        DeathknightBlood = 250,
        DeathknightFrost = 251,
        DeathknightUnholy = 252,
        HunterBeastmaster = 253,
        HunterMarksman = 254,
        HunterSurvival = 255,
        PriestDiscipline = 256,
        PriestHoly = 257,
        PriestShadow = 258,
        RogueAssassination = 259,
        RogueCombat = 260,
        RogueSubtlety = 261,
        ShamanElemental = 262,
        ShamanEnhancement = 263,
        ShamanRestoration = 264,
        WarlockAffliction = 265,
        WarlockDemonology = 266,
        WarlockDestruction = 267,
        MonkBrewmaster = 268,
        MonkBattledancer = 269,
        MonkMistweaver = 270
    }

    public enum SpecResetType
    {
        Talents = 0,
        Specialization = 1,
        Glyphs = 2,
        PetTalents = 3
    }

    public enum PlayerCurrencyState
    {
        Unchanged = 0,
        Changed = 1,
        New = 2,
        Removed = 3
    }

    public enum CUFBoolOptions
    {
        KeepGroupsTogether,
        DisplayPets,
        DisplayMainTankAndAssist,
        DisplayHealPrediction,
        DisplayAggroHighlight,
        DisplayOnlyDispellableDebuffs,
        DisplayPowerBar,
        DisplayBorder,
        UseClassColors,
        DisplayHorizontalGroups,
        DisplayNonBossDebuffs,
        DynamicPosition,
        Locked,
        Shown,
        AutoActivate2Players,
        AutoActivate3Players,
        AutoActivate5Players,
        AutoActivate10Players,
        AutoActivate15Players,
        AutoActivate25Players,
        AutoActivate40Players,
        AutoActivateSpec1,
        AutoActivateSpec2,
        AutoActivateSpec3,
        AutoActivateSpec4,
        AutoActivatePvp,
        AutoActivatePve,

        BoolOptionsCount,
    }

    public enum TrainerSpellState
    {
        Gray = 0,
        Green = 1,
        Red = 2,
        GreenDisabled = 10     // custom value, not send to client: formally green but learn not allowed
    }

    public enum ActionButtonUpdateState
    {
        Unchanged = 0,
        Changed = 1,
        New = 2,
        Deleted = 3
    }

    public enum ActionButtonType
    {
        Spell = 0x00,
        Click = 0x01,
        Eqset = 0x20,
        Dropdown = 0x30,
        Macro = 0x40,
        ClickMacro = Click | Macro,
        Mount = 0x60,
        Item = 0x80
    }

    public enum ReputationSource
    {
        Kill,
        Quest,
        DailyQuest,
        WeeklyQuest,
        MonthlyQuest,
        RepeatableQuest,
        Spell
    }

    public enum RuneCooldowns
    {
        RuneBaseCooldown = 10000,
        RuneMissCooldown = 1500     // cooldown applied on runes when the spell misses
    }

    public enum DrunkenState
    {
        Sober = 0,
        Tipsy = 1,
        Drunk = 2,
        Smashed = 3
    }

    public enum PlayerFlags
    {
        GroupLeader = 0x00000001,
        Afk = 0x00000002,
        Dnd = 0x00000004,
        Gm = 0x00000008,
        Ghost = 0x00000010,
        Resting = 0x00000020,
        ContestedPvp = 0x00000100,             // Player has been involved in a PvP combat and will be attacked by contested guards
        InPvp = 0x00000200,
        HideHelm = 0x00000400,
        HideCloak = 0x00000800,
        PlayedLongTime = 0x00001000,          // played long time
        PlayedTooLong = 0x00002000,           // played too long time
        IsOutOfBounds = 0x00004000,
        Developer = 0x00008000,                 // <Dev> prefix for something?
        TaxiBenchmark = 0x00020000,            // taxi benchmark mode (on/off) (2.0.1)
        PvpTimer = 0x00040000,                 // 3.0.2, pvp timer active (after you disable pvp manually)
        Uber = 0x00080000,
        ReagentBankUnlocked = 0x00100000,
        MercenaryMode = 0x00200000,
        Commentator2 = 0x00400000,
        AllowOnlyAbility = 0x00800000,        // used by bladestorm and killing spree, allowed only spells with SPELL_ATTR0_REQ_AMMO, SPELL_EFFECT_ATTACK, checked only for active player
        PetBattlesUnlocked = 0x01000000,      // enables pet battles
        NoXPGain = 0x02000000,
        AutoDeclineGuild = 0x08000000,        // Automatically declines guild invites
        GuildLevelEnabled = 0x10000000,       // Lua_GetGuildLevelEnabled() - enables guild leveling related UI
        VoidUnlocked = 0x20000000,             // void storage
        Mentor = 0x40000000,
    }

    public enum PlayerLocalFlags
    {
        TrackStealthed = 0x00000002,
        ReleaseTimer = 0x00000008,         // Display time till auto release spirit
        NoReleaseWindow = 0x00000010,     // Display no "release spirit" window at all
        NoPetBar = 0x00000020,            // CGPetInfo::IsPetBarUsed
        OverrideCameraMinHeight = 0x00000040,
        UsingPartyGarrison = 0x00000100,
        CanUseObjectsMounted = 0x00000200,
        CanVisitPartyGarrison = 0x00000400
    }

    public enum PlayerBytesOffsets
    {
        SkinID = 0,
        FaceID = 1,
        HairStyleID = 2,
        HairColorID = 3
    }

    public enum PlayerBytes2Offsets
    {
        CustomDisplayOption = 0, // 3 bytes
        FacialStyle = 3,
    }

    public enum PlayerBytes3Offsets
    {
        PartyType = 0,
        BankBagSlots = 1,
        Gender = 2,
        Inebriation = 3,
    }

    public enum PlayerBytes4Offsets
    {
        PvpTitle = 0,
        ArenaFaction = 1
    }

    public enum PlayerFieldBytesOffsets
    {
        RafGrantableLevel = 0,
        ActionBarToggles = 1,
        LifetimeMaxPvpRank = 2,
        MaxArtifactPowerRanks = 3,
    }

    public enum PlayerFieldBytes2Offsets
    {
        IgnorePowerRegenPredictionMask = 0,
        AuraVision = 1,
    }

    public enum PlayerFieldKillsOffsets
    {
        TodayKills = 0,
        YesterdayKills = 1
    }

    public enum PlayerRestInfoOffsets
    {
        StateXP = 0,
        RestedXP = 1,
        StateHonor = 2,
        RestedHonor = 3,

        MaxRestInfo
    }

    public enum MirrorTimerType
    {
        FatigueTimer = 0,
        BreathTimer = 1,
        FireTimer = 2 // feign death
    }

    [Flags]
    public enum PlayerFieldByte2Flags
    {
        None = 0x00,
        Stealth = 0x20,
        InvisibilityGlow = 0x40
    }

    [Flags]
    public enum PlayerExtraFlags
    {
        // gm abilities
        GmOn = 0x0001,
        AcceptWhispers = 0x0004,
        Taxicheat = 0x0008,
        Invisible = 0x0010,
        GmChat = 0x0020,               // Show GM badge in chat messages

        // other states
        PvpDeath = 0x0100                // store PvP death status until corpse creating.
    }

    [Flags]
    public enum AtLoginFlags
    {
        None = 0x000,
        Rename = 0x001,
        ResetSpells = 0x002,
        ResetTalents = 0x004,
        Customize = 0x008,
        ResetPetTalents = 0x010,
        First = 0x020,
        ChangeFaction = 0x040,
        ChangeRace = 0x080,
        Resurrect = 0x100,
    }

    public enum QuestSaveType
    {
        DefaultSaveType = 0,
        DeleteSaveType,
        ForceDeleteSaveType
    }

    public enum QuestSlotOffsets
    {
        IDOffset = 0,
        StateOffset = 1,
        CountsOffset = 2,
        TimeOffset = 14
    }

    public enum QuestSlotStateMask
    {
        None = 0x0000,
        Complete = 0x0001,
        Fail = 0x0002
    }

    public enum SkillUpdateState
    {
        Unchanged = 0,
        Changed = 1,
        New = 2,
        Deleted = 3
    }

    public enum AttackSwingErr
    {
        CantAttack = 0,
        BadFacing = 1,
        NotInRange = 2,
        DeadTarget = 3
    }

    public enum PlayerSlots
    {
        SlotStart = 0,// first slot for item stored (in any way in player m_items data)
        SlotEnd = 187,// last+1 slot for item stored (in any way in player m_items data)
        SlotsCount = (SlotEnd - SlotStart)
    }

    public enum EquipmentSlots : byte
    {
        Head = 0,
        Neck = 1,
        Shoulders = 2,
        Body = 3,
        Chest = 4,
        Waist = 5,
        Legs = 6,
        Feet = 7,
        Wrists = 8,
        Hands = 9,
        Finger1 = 10,
        Finger2 = 11,
        Trinket1 = 12,
        Trinket2 = 13,
        Back = 14,
        Mainhand = 15,
        Offhand = 16,
        Ranged = 17,
        Tabard = 18,

        Max = 19
    }

    public enum InventorySlots : byte           // 4 slots
    {
        BagStart = 19,
        BagEnd = 23
    }

    public enum InventoryPackSlots : byte       // 16 slots
    {
        ItemStart = 23,
        ItemEnd = 39
    }

    public enum BankItemSlots                   // 28 slots
    {
        ItemStart = 39,
        ItemEnd = 67
    }

    public enum BankBagSlots                    // 7 slots
    {
        BagStart = 67,
        BagEnd = 74
    }

    public enum BuyBackSlots                    // 12 slots
    {
        // stored in m_buybackitems
        SlotStart = 74,
        SlotEnd = 86
    }

    public enum ReagentSlots
    {
        SlotStart = 87,
        SlotEnd = 184,
    }

    public enum ChildEquipmentSlots
    {
        SlotStart = 184,
        SlotEnd = 187,
    }

    public enum EquipmentSetUpdateState
    {
        Unchanged = 0,
        Changed = 1,
        New = 2,
        Deleted = 3
    }

    public enum EquipmentSetType
    {
        Equipment = 0,
        Transmog = 1
    }

    public enum TransferAbortReason
    {
        None = 0,
        Error = 1,
        MaxPlayers = 2,   // Transfer Aborted: instance is full
        NotFound = 3,   // Transfer Aborted: instance not found
        TooManyInstances = 4,   // You have entered too many instances recently.
        ZoneInCombat = 6,   // Unable to zone in while an encounter is in progress.
        InsufExpanLvl = 7,   // You must have <TBC, WotLK> expansion installed to access this area.
        Difficulty = 8,   // <Normal, Heroic, Epic> difficulty mode is not available for %s.
        UniqueMessage = 9,   // Until you've escaped TLK's grasp, you cannot leave this place!
        TooManyRealmInstances = 10,  // Additional instances cannot be launched, please try again later.
        NeedGroup = 11,  // Transfer Aborted: you must be in a raid group to enter this instance
        NotFound2 = 12,  // Transfer Aborted: instance not found
        NotFound3 = 13,  // Transfer Aborted: instance not found
        NotFound4 = 14,  // Transfer Aborted: instance not found
        RealmOnly = 15,  // All players in the party must be from the same realm to enter %s.
        MapNotAllowed = 16,  // Map cannot be entered at this time.
        LockedToDifferentInstance = 18,  // You are already locked to %s
        AlreadyCompletedEncounter = 19,  // You are ineligible to participate in at least one encounter in this instance because you are already locked to an instance in which it has been defeated.
        DifficultyNotFound = 22,  // client writes to console "Unable to resolve requested difficultyID %u to actual difficulty for map %d"
        XrealmZoneDown = 24,  // Transfer Aborted: cross-realm zone is down
        SoloPlayerSwitchDifficulty = 26,  // This instance is already in progress. You may only switch difficulties from inside the instance.
    }

    public enum NewWorldReason
    {
        Normal = 16,    // Normal map change
        Seamless = 21   // Teleport to another map without a loading screen, used for outdoor scenarios
    }

    public enum InstanceResetWarningType
    {
        WarningHours = 1,                  // WARNING! %s is scheduled to reset in %d hour(s).
        WarningMin = 2,                    // WARNING! %s is scheduled to reset in %d minute(s)!
        WarningMinSoon = 3,               // WARNING! %s is scheduled to reset in %d minute(s). Please exit the zone or you will be returned to your bind location!
        Welcome = 4,                        // Welcome to %s. This raid instance is scheduled to reset in %s.
        Expired = 5
    }

    public enum ArenaTeamInfoType
    {
        ID = 0,
        Type = 1,
        Member = 2, // 0 - captain, 1 - member
        GamesWeek = 3,
        GamesSeason = 4,
        WinsSeason = 5,
        PersonalRating = 6,
    }

    public enum RestFlag
    {
        InTavern = 0x1,
        InCity = 0x2,
        InFactionArea = 0x4, // used with AREA_FLAG_REST_ZONE_*
    }

    public enum TeleportToOptions
    {
        GmMode = 0x01,
        NotLeaveTransport = 0x02,
        NotLeaveCombat = 0x04,
        NotUnsummonPet = 0x08,
        Spell = 0x10,
        Seamless = 0x20
    }

    public enum EnviromentalDamage : byte
    {
        Exhausted = 0,
        Drowning = 1,
        Fall = 2,
        Lava = 3,
        Slime = 4,
        Fire = 5,
        FallToVoid = 6                                 // custom case for fall without durability loss
    }

    public enum PlayedTimeIndex
    {
        Total = 0,
        Level = 1
    }

// used at player loading query list preparing, and later result selection
    public enum PlayerLoginQueryIndex
    {
        LoadFrom,
        LoadGroup,
        LoadBoundInstances,
        LoadAuras,
        LoadAuraEffects,
        LoadSpells,
        LoadQuestStatus,
        LoadQuestStatusObjectives,
        LoadDailyQuestStatus,
        LoadReputation,
        LoadInventory,
        LoadArtifacts,
        LoadActions,
        LoadMailCount,
        LoadMailDate,
        LoadSocialList,
        LoadHomeBind,
        LoadSpellCooldowns,
        LoadSpellCharges,
        LoadDeclinedNames,
        LoadGuild,
        LoadArenaInfo,
        LoadAchievements,
        LoadCriteriaProgress,
        LoadEquipmentSets,
        LoadTransmogOutfits,
        LoadBGData,
        LoadGlyphs,
        LoadTalents,
        LoadAccountData,
        LoadSkills,
        LoadWeeklyQuestStatus,
        LoadRandomBG,
        LoadBanned,
        LoadQuestStatusRew,
        LoadInstanceLockTimes,
        LoadSeasonalQuestStatus,
        LoadMonthlyQuestStatus,
        LoadVoidStorage,
        LoadCurrency,
        LoadCUFProfiles,
        LoadCorpseLocation,
        LoadGarrison,
        LoadGarrisonBlueprints,
        LoadGarrisonBuildings,
        LoadGarrisonFollowers,
        LoadGarrisonFollowerAbilities,
        MaxPlayerLoginQuery
    }

    [Flags]
    public enum PlayerDelayedOperations
    {
        SavePlayer = 0x01,
        ResurrectPlayer = 0x02,
        SpellCastDeserter = 0x04,
        BGMountRestore = 0x08,                    // Flag to restore mount state after teleport from BG
        BGTaxiRestore = 0x10,                     // Flag to restore taxi state after teleport from BG
        BGGroupRestore = 0x20,                    // Flag to restore group state after teleport from BG
    }

    public enum BindExtensionState
    {
        /// <summary>
        /// Doesn't affect anything unless manually re-extended by player.
        /// </summary>
        Expired = 0,
        /// <summary>
        /// Standard state.
        /// </summary>
        Normal = 1,
        /// <summary>
        /// Won't be promoted to Expired at next reset period, will instead be promoted to Normal.
        /// </summary>
        Extended = 2,
        /// <summary>
        /// Special state: keep current save type.
        /// </summary>
        Keep = 255
    }

    public enum CharDeleteMethod
    {
        Remove = 0,                     // Completely remove from the database
        Unlink = 1                      // The character gets unlinked from the account,
        // the name gets freed up and appears as deleted ingame
    }

    public enum ReferAFriendError
    {
        None = 0,
        NotReferredBy = 1,
        TargetTooHigh = 2,
        InsufficientGrantableLevels = 3,
        TooFar = 4,
        DifferentFaction = 5,
        NotNow = 6,
        GrantLevelMaxI = 7,
        NoTarget = 8,
        NotInGroup = 9,
        SummonLevelMaxI = 10,
        SummonCooldown = 11,
        InsufExpanLvl = 12,
        SummonOfflineS = 13,
        NoXrealm = 14,
        MapIncomingTransferNotAllowed = 15
    }

    [Flags]
    public enum PlayerRestState : byte
    {
        Rested = 0x01,
        NotRafLinked = 0x02,
        RafLinked = 0x06
    }

    [Flags]
    public enum PlayerCommandStates
    {
        CheatNone = 0x00,
        CheatGod = 0x01,
        CheatCasttime = 0x02,
        CheatCooldown = 0x04,
        CheatPower = 0x08,
        CheatWaterwalk = 0x10
    }

    public enum PlayerLogXPReason : byte
    {
        Kill = 0,
        NoKill = 1
    }

    public enum FriendStatus
    {
        Offline = 0x00,
        Online = 0x01,
        AFK = 0x02,
        DND = 0x04,
        Raf = 0x08
    }

    [Flags]
    public enum SocialFlag
    {
        Friend = 0x01,
        Ignored = 0x02,
        Muted = 0x04,
    }

    /// <summary>
    /// Results of friend related commands.
    /// </summary>
    public enum FriendsResult : byte
    {
        DBError = 0x00,
        ListFull = 0x01,
        Online = 0x02,
        Offline = 0x03,
        NotFound = 0x04,
        Removed = 0x05,
        AddedOnline = 0x06,
        AddedOffline = 0x07,
        Already = 0x08,
        Self = 0x09,
        Enemy = 0x0A,
        IgnoreFull = 0x0B,
        IgnoreSelf = 0x0C,
        IgnoreNotFound = 0x0D,
        IgnoreAlready = 0x0E,
        IgnoreAdded = 0x0F,
        IgnoreRemoved = 0x10,
        IgnoreAmbiguous = 0x11,        // That name is ambiguous, type more of the player's server name
        MuteFull = 0x12,
        MuteSelf = 0x13,
        MuteNotFound = 0x14,
        MuteAlready = 0x15,
        MuteAdded = 0x16,
        MuteRemoved = 0x17,
        MuteAmbiguous = 0x18,          // That name is ambiguous, type more of the player's server name
    }

    public enum TalentLearnResult
    {
        Ok = 0,
        Unknown = 1,
        NotEnoughTalentsInPrimaryTree = 2,
        NoPrimaryTreeSelected = 3,
        CantDoThatRightNow = 4,
        AffectingCombat = 5,
        CantRemoveTalent = 6,
        CantDoThatChallengeModeActive = 7,
        RestArea = 8
    }
}