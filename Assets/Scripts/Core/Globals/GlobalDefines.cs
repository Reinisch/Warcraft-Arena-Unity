namespace Core
{
    public enum ScriptsType
    {
        Spell = 1,
        Event,
        Waypoint,
    }

    public enum ScriptFlags
    {
        // Talk Flags
        TalkUsePlayer = 0x1,

        // Emote flags
        EmoteUseState = 0x1,

        // TeleportTo flags
        TeleportUseCreature = 0x1,

        // KillCredit flags
        KillcreditRewardGroup = 0x1,

        // RemoveAura flags
        RemoveauraReverse = 0x1,

        // CastSpell flags
        CastspellSourceToTarget = 0,
        CastspellSourceToSource = 1,
        CastspellTargetToTarget = 2,
        CastspellTargetToSource = 3,
        CastspellSearchCreature = 4,

        CastspellTriggered = 0x1,

        // PlaySound flags
        PlaysoundTargetPlayer = 0x1,
        PlaysoundDistanceSound = 0x2,

        // Orientation flags
        OrientationFaceTarget = 0x1
    }

    /// <summary>
    /// DB scripting commands.
    /// </summary>
    public enum ScriptCommands
    {
        Talk = 0,                   // source/target = Creature, target = any, datalong = talk type (0=say, 1=whisper, 2=yell, 3=emote text, 4=boss emote text), datalong2 & 1 = player talk (instead of creature), dataint = string_id
        Emote = 1,                  // source/target = Creature, datalong = emote id, datalong2 = 0: set emote state; > 0: play emote state
        FieldSet = 2,               // source/target = Creature, datalong = field id, datalog2 = value
        MoveTo = 3,                 // source/target = Creature, datalong2 = time to reach, x/y/z = destination
        FlagSet = 4,                // source/target = Creature, datalong = field id, datalog2 = bitmask
        FlagRemove = 5,             // source/target = Creature, datalong = field id, datalog2 = bitmask
        TeleportTo = 6,             // source/target = Creature/Player (see datalong2), datalong = map_id, datalong2 = 0: Player; 1: Creature, x/y/z = destination, o = orientation
        QuestExplored = 7,          // target/source = Player, target/source = GO/Creature, datalong = quest id, datalong2 = distance or 0
        KillCredit = 8,             // target/source = Player, datalong = creature entry, datalong2 = 0: personal credit, 1: group credit
        RespawnGameEntity = 9,      // source = WorldObject (summoner), datalong = GO guid, datalong2 = despawn delay
        TempSummonCreature = 10,    // source = WorldObject (summoner), datalong = creature entry, datalong2 = despawn delay, x/y/z = summon position, o = orientation
        OpenDoor = 11,              // source = Unit, datalong = GO guid, datalong2 = reset delay (min 15)
        CloseDoor = 12,             // source = Unit, datalong = GO guid, datalong2 = reset delay (min 15)
        ActivateEntity = 13,        // source = Unit, target = GO
        RemoveAura = 14,            // source (datalong2 != 0) or target (datalong2 == 0) = Unit, datalong = spell id
        CastSpell = 15,             // source and/or target = Unit, datalong2 = cast direction (0: s->t 1: s->s 2: t->t 3: t->s 4: s->creature with dataint entry), dataint & 1 = triggered flag
        PlaySound = 16,             // source = WorldObject, target = none/Player, datalong = sound id, datalong2 (bitmask: 0/1=anyone/player, 0/2=without/with distance dependency, so 1|2 = 3 is target with distance dependency)
        CreateItem = 17,            // target/source = Player, datalong = item entry, datalong2 = amount
        DespawnSelf = 18,           // target/source = Creature, datalong = despawn delay

        LoadPath = 20,              // source = Unit, datalong = path id, datalong2 = is repeatable
        CallScriptToUnit = 21,      // source = WorldObject (if present used as a search center), datalong = script id, datalong2 = unit lowguid, dataint = script table to use (see ScriptsType)
        Kill = 22,                  // source/target = Creature, dataint = remove corpse attribute

        // TrinityCore only
        Orientation = 30,           // source = Unit, target (datalong > 0) = Unit, datalong = > 0 turn source to face target, o = orientation
        Equip = 31,                 // soucre = Creature, datalong = equipment id
        Model = 32,                 // source = Creature, datalong = model id
        CloseGossip = 33,           // source = Player
        PlayMovie = 34,             // source = Player, datalong = movie id
        PlayAnimKit = 35            // source = Creature, datalong = AnimKit id
    }
}