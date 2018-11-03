/*  Documentation on implementing a new ConditionType:
    Step 1: Check for the lowest free ID. Look for CONDITION_UNUSED_XX in the enum. Then define the new condition type.
    Step 2: Determine and map the parameters for the new condition type.
    Step 3: Add a case block to ConditionMgr::isConditionTypeValid with the new condition type  and validate the parameters.
    Step 4: Define the maximum available condition targets in ConditionMgr::GetMaxAvailableConditionTargets.
    Step 5: Define the grid searcher mask in Condition::GetSearcherTypeMaskForCondition.
    Step 6: Add a case block to ConditionMgr::Meets with the new condition type.
    Step 7: Define condition name and expected condition values in ConditionMgr::StaticConditionTypeData. */

namespace Core
{
    public enum ConditionTypes
    {                               // value1           value2         value3
        None = 0,                   // 0                0              0                  always true
        Aura = 1,                   // spell_id         effindex       0                  true if target has aura of spell_id with effect effindex
        Item = 2,                   // item_id          count          bank               true if has #count of item_ids (if 'bank' is set it searches in bank slots too)
        ItemEquipped = 3,           // item_id          0              0                  true if has item_id equipped
        Zoneid = 4,                 // zone_id          0              0                  true if in zone_id
        ReputationRank = 5,         // faction_id       rankMask       0                  true if has min_rank for faction_id
        Team = 6,                   // player_team      0,             0                  469 - Alliance, 67 - Horde)
        Skill = 7,                  // skill_id         skill_value    0                  true if has skill_value for skill_id
        Questrewarded = 8,          // quest_id         0              0                  true if quest_id was rewarded before
        Questtaken = 9,             // quest_id         0,             0                  true while quest active
        Drunkenstate = 10,          // DrunkenState     0,             0                  true if player is drunk enough
        WorldState = 11,            // index            value          0                  true if world has the value for the index
        ActiveEvent = 12,           // event_id         0              0                  true if event is active
        InstanceInfo = 13,          // entry            data           type               true if the instance info defined by type (enum InstanceInfo) equals data.
        QuestNone = 14,             // quest_id         0              0                  true if doesn't have quest saved
        Class = 15,                 // class            0              0                  true if player's class is equal to class
        Race = 16,                  // race             0              0                  true if player's race is equal to race
        Achievement = 17,           // achievement_id   0              0                  true if achievement is complete
        Title = 18,                 // title id         0              0                  true if player has title
        Spawnmask = 19,             // spawnMask        0              0                  true if in spawnMask
        Gender = 20,                // gender           0              0                  true if player's gender is equal to gender
        UnitState = 21,             // unitState        0              0                  true if unit has unitState
        Mapid = 22,                 // map_id           0              0                  true if in map_id
        Areaid = 23,                // area_id          0              0                  true if in area_id
        CreatureType = 24,          // cinfo.type       0              0                  true if creature_template.type = value1
        Spell = 25,                 // spell_id         0              0                  true if player has learned spell
        Phaseid = 26,               // phaseid          0              0                  true if object is in phaseid
        Level = 27,                 // level            ComparisonType 0                  true if unit's level is equal to param1 (param2 can modify the statement)
        QuestComplete = 28,         // quest_id         0              0                  true if player has quest_id with all objectives complete, but not yet rewarded
        NearCreature = 29,          // creature entry   distance       dead (0/1)         true if there is a creature of entry in range
        NearGameobject = 30,        // gameobject entry distance       0                  true if there is a gameobject of entry in range
        ObjectEntryGUID = 31,       // TypeID           entry          guid               true if object is type TypeID and the entry is 0 or matches entry of the object or matches guid of the object
        TypeMask = 32,              // TypeMask         0              0                  true if object is type object's TypeMask matches provided TypeMask
        RelationTo = 33,            // ConditionTarget  RelationType   0                  true if object is in given relation with object specified by ConditionTarget
        ReactionTo = 34,            // ConditionTarget  rankMask       0                  true if object's reaction matches rankMask object specified by ConditionTarget
        DistanceTo = 35,            // ConditionTarget  distance       ComparisonType     true if object and ConditionTarget are within distance given by parameters
        Alive = 36,                 // 0                0              0                  true if unit is alive
        HpVal = 37,                 // hpVal            ComparisonType 0                  true if unit's hp matches given value
        HpPct = 38,                 // hpPct            ComparisonType 0                  true if unit's hp matches given pct
        RealmAchievement = 39,      // achievement_id   0              0                  true if realm achievement is complete
        InWater = 40,               // 0                0              0                  true if unit in water
        TerrainSwap = 41,           // terrainSwap      0              0                  true if object is in terrainswap
        StandState = 42,            // stateType        state          0                  true if unit matches specified sitstate (0,x: has exactly state x; 1,0: any standing state; 1,1: any sitting state;)
    }

/*  Documentation on implementing a new ConditionSourceType:
    Step 1: Check for the lowest free ID. Look for CONDITION_SOURCE_TYPE_UNUSED_XX in the enum. Then define the new source type.
    Step 2: Determine and map the parameters for the new condition source type.
    Step 3: Add a case block to ConditionMgr::isSourceTypeValid with the new condition type and validate the parameters.
    Step 4: If your condition can be grouped (determined in step 2), add a rule for it in ConditionMgr::CanHaveSourceGroupSet, following the example of the existing types.
    Step 5: Define the maximum available condition targets in ConditionMgr::GetMaxAvailableConditionTargets.
    Step 6: Define ConditionSourceType Name in ConditionMgr::StaticSourceTypeData.

    The following steps only apply if your condition can be grouped:
    Step 7: Determine how you are going to store your conditions. You need to add a new storage container
            for it in ConditionMgr class, along with a function like: ConditionList GetConditionsForXXXYourNewSourceTypeXXX(parameters...)
            The above function should be placed in upper level (practical) code that actually checks the conditions.
    Step 8: Implement loading for your source type in ConditionMgr::LoadConditions.
    Step 9: Implement memory cleaning for your source type in ConditionMgr::Clean. */
    public enum ConditionSourceType
    {
        None = 0,
        CreatureLootTemplate = 1,
        DisenchantLootTemplate = 2,
        FishingLootTemplate = 3,
        GameobjectLootTemplate = 4,
        ItemLootTemplate = 5,
        MailLootTemplate = 6,
        MillingLootTemplate = 7,
        PickpocketingLootTemplate = 8,
        ProspectingLootTemplate = 9,
        ReferenceLootTemplate = 10,
        SkinningLootTemplate = 11,
        SpellLootTemplate = 12,
        SpellImplicitTarget = 13,
        GossipMenu = 14,
        GossipMenuOption = 15,
        CreatureTemplateVehicle = 16,
        Spell = 17,
        SpellClickEvent = 18,
        QuestAccept = 19,
        QuestShowMark = 20,
        VehicleSpell = 21,
        SmartEvent = 22,
        NPCVendor = 23,
        SpellProc = 24,
        TerrainSwap = 25,
        Phase = 26,
        Max = 27
    }

    public enum RelationType
    {
        Self = 0,
        InParty,
        InRaidOrParty,
        OwnedBy,
        PassengerOf,
        CreatedBy,
        Max
    }

    public enum InstanceInfo
    {
        Data = 0,
        GUIDData,
        BossState,
        Data64
    }
}