using UnityEngine;
using System.Collections;

public static class SpellExtensions
{
    private struct StaticTargetData
    {
        public TargetObjects ObjectType;    // type of object returned by target type
        public TargetReferences ReferenceType; // defines which object is used as a reference when selecting target
        public TargetSelections SelectionCategory;
        public TargetChecks SelectionCheckType; // defines selection criteria
        public TargetDirections DirectionType; // direction for cone and dest targets

        public StaticTargetData(TargetObjects objectType, TargetReferences referenceType,
            TargetSelections selectionCategory, TargetChecks selectionCheckType, TargetDirections directionType)
        {
            ObjectType = objectType;
            ReferenceType = referenceType;
            SelectionCategory = selectionCategory;
            SelectionCheckType = selectionCheckType;
            DirectionType = directionType;
        }
    };

    #region All target types
    // #TODO : Change to dictionary
    private static StaticTargetData[] targetTypeData = new StaticTargetData[(int)TargetTypes.TOTAL_SPELL_TARGETS]
    {
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        //
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 1 TARGET_UNIT_CASTER
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.NEARBY,  TargetChecks.ENEMY,    TargetDirections.NONE),        // 2 TARGET_UNIT_NEARBY_ENEMY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.NEARBY,  TargetChecks.PARTY,    TargetDirections.NONE),        // 3 TARGET_UNIT_NEARBY_PARTY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.NEARBY,  TargetChecks.ALLY,     TargetDirections.NONE),        // 4 TARGET_UNIT_NEARBY_ALLY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 5 TARGET_UNIT_PET
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.ENEMY,    TargetDirections.NONE),        // 6 TARGET_UNIT_TARGET_ENEMY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.SRC,    TargetSelections.AREA,    TargetChecks.ENTRY,    TargetDirections.NONE),        // 7 TARGET_UNIT_SRC_AREA_ENTRY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.DEST,   TargetSelections.AREA,    TargetChecks.ENTRY,    TargetDirections.NONE),        // 8 TARGET_UNIT_DEST_AREA_ENTRY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 9 TARGET_DEST_HOME
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 10
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.SRC,    TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 11 TARGET_UNIT_SRC_AREA_UNK_11
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 12
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 13
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 14
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.SRC,    TargetSelections.AREA,    TargetChecks.ENEMY,    TargetDirections.NONE),        // 15 TARGET_UNIT_SRC_AREA_ENEMY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.DEST,   TargetSelections.AREA,    TargetChecks.ENEMY,    TargetDirections.NONE),        // 16 TARGET_UNIT_DEST_AREA_ENEMY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 17 TARGET_DEST_DB
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 18 TARGET_DEST_CASTER
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 19
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.AREA,    TargetChecks.PARTY,    TargetDirections.NONE),        // 20 TARGET_UNIT_CASTER_AREA_PARTY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.ALLY,     TargetDirections.NONE),        // 21 TARGET_UNIT_TARGET_ALLY
        new StaticTargetData(TargetObjects.SRC,  TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 22 TARGET_SRC_CASTER
        new StaticTargetData(TargetObjects.GOBJ, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 23 TARGET_GAMEOBJECT_TARGET
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.CONE,    TargetChecks.ENEMY,    TargetDirections.FRONT),       // 24 TARGET_UNIT_CONE_ENEMY_24
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 25 TARGET_UNIT_TARGET_ANY
        new StaticTargetData(TargetObjects.GOBJ_ITEM, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT, TargetDirections.NONE),    // 26 TARGET_GAMEOBJECT_ITEM_TARGET
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 27 TARGET_UNIT_MASTER
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.ENEMY,    TargetDirections.NONE),        // 28 TARGET_DEST_DYNOBJ_ENEMY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.ALLY,     TargetDirections.NONE),        // 29 TARGET_DEST_DYNOBJ_ALLY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.SRC,    TargetSelections.AREA,    TargetChecks.ALLY,     TargetDirections.NONE),        // 30 TARGET_UNIT_SRC_AREA_ALLY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.DEST,   TargetSelections.AREA,    TargetChecks.ALLY,     TargetDirections.NONE),        // 31 TARGET_UNIT_DEST_AREA_ALLY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT_LEFT),  // 32 TARGET_DEST_CASTER_SUMMON
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.SRC,    TargetSelections.AREA,    TargetChecks.PARTY,    TargetDirections.NONE),        // 33 TARGET_UNIT_SRC_AREA_PARTY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.DEST,   TargetSelections.AREA,    TargetChecks.PARTY,    TargetDirections.NONE),        // 34 TARGET_UNIT_DEST_AREA_PARTY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.PARTY,    TargetDirections.NONE),        // 35 TARGET_UNIT_TARGET_PARTY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 36 TARGET_DEST_CASTER_UNK_36
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.LAST,   TargetSelections.AREA,    TargetChecks.PARTY,    TargetDirections.NONE),        // 37 TARGET_UNIT_LASTTARGET_AREA_PARTY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.NEARBY,  TargetChecks.ENTRY,    TargetDirections.NONE),        // 38 TARGET_UNIT_NEARBY_ENTRY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 39 TARGET_DEST_CASTER_FISHING
        new StaticTargetData(TargetObjects.GOBJ, TargetReferences.CASTER, TargetSelections.NEARBY,  TargetChecks.ENTRY,    TargetDirections.NONE),        // 40 TARGET_GAMEOBJECT_NEARBY_ENTRY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT_RIGHT), // 41 TARGET_DEST_CASTER_FRONT_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK_RIGHT),  // 42 TARGET_DEST_CASTER_BACK_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK_LEFT),   // 43 TARGET_DEST_CASTER_BACK_LEFT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT_LEFT),  // 44 TARGET_DEST_CASTER_FRONT_LEFT
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.ALLY,     TargetDirections.NONE),        // 45 TARGET_UNIT_TARGET_CHAINHEAL_ALLY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.NEARBY,  TargetChecks.ENTRY,    TargetDirections.NONE),        // 46 TARGET_DEST_NEARBY_ENTRY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT),       // 47 TARGET_DEST_CASTER_FRONT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK),        // 48 TARGET_DEST_CASTER_BACK
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RIGHT),       // 49 TARGET_DEST_CASTER_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.LEFT),        // 50 TARGET_DEST_CASTER_LEFT
        new StaticTargetData(TargetObjects.GOBJ, TargetReferences.SRC,    TargetSelections.AREA,    TargetChecks.DEFAULT,  TargetDirections.NONE),        // 51 TARGET_GAMEOBJECT_SRC_AREA
        new StaticTargetData(TargetObjects.GOBJ, TargetReferences.DEST,   TargetSelections.AREA,    TargetChecks.DEFAULT,  TargetDirections.NONE),        // 52 TARGET_GAMEOBJECT_DEST_AREA
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.ENEMY,    TargetDirections.NONE),        // 53 TARGET_DEST_TARGET_ENEMY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.CONE,    TargetChecks.ENEMY,    TargetDirections.FRONT),       // 54 TARGET_UNIT_CONE_ENEMY_54
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 55 TARGET_DEST_CASTER_FRONT_LEAP
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.AREA,    TargetChecks.RAID,     TargetDirections.NONE),        // 56 TARGET_UNIT_CASTER_AREA_RAID
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.RAID,     TargetDirections.NONE),        // 57 TARGET_UNIT_TARGET_RAID
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.NEARBY,  TargetChecks.RAID,     TargetDirections.NONE),        // 58 TARGET_UNIT_NEARBY_RAID
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.CONE,    TargetChecks.ALLY,     TargetDirections.FRONT),       // 59 TARGET_UNIT_CONE_ALLY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.CONE,    TargetChecks.ENTRY,    TargetDirections.FRONT),       // 60 TARGET_UNIT_CONE_ENTRY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.AREA,    TargetChecks.RAID_CLASS, TargetDirections.NONE),      // 61 TARGET_UNIT_TARGET_AREA_RAID_CLASS
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 62 TARGET_UNK_62
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 63 TARGET_DEST_TARGET_ANY
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT),       // 64 TARGET_DEST_TARGET_FRONT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK),        // 65 TARGET_DEST_TARGET_BACK
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RIGHT),       // 66 TARGET_DEST_TARGET_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.LEFT),        // 67 TARGET_DEST_TARGET_LEFT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT_RIGHT), // 68 TARGET_DEST_TARGET_FRONT_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK_RIGHT),  // 69 TARGET_DEST_TARGET_BACK_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK_LEFT),   // 70 TARGET_DEST_TARGET_BACK_LEFT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT_LEFT),  // 71 TARGET_DEST_TARGET_FRONT_LEFT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RANDOM),      // 72 TARGET_DEST_CASTER_RANDOM
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RANDOM),      // 73 TARGET_DEST_CASTER_RADIUS
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RANDOM),      // 74 TARGET_DEST_TARGET_RANDOM
        new StaticTargetData(TargetObjects.DEST, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RANDOM),      // 75 TARGET_DEST_TARGET_RADIUS
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.CHANNEL, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 76 TARGET_DEST_CHANNEL_TARGET
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.CHANNEL, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 77 TARGET_UNIT_CHANNEL_TARGET
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT),       // 78 TARGET_DEST_DEST_FRONT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK),        // 79 TARGET_DEST_DEST_BACK
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RIGHT),       // 80 TARGET_DEST_DEST_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.LEFT),        // 81 TARGET_DEST_DEST_LEFT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT_RIGHT), // 82 TARGET_DEST_DEST_FRONT_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK_RIGHT),  // 83 TARGET_DEST_DEST_BACK_RIGHT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.BACK_LEFT),   // 84 TARGET_DEST_DEST_BACK_LEFT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.FRONT_LEFT),  // 85 TARGET_DEST_DEST_FRONT_LEFT
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RANDOM),      // 86 TARGET_DEST_DEST_RANDOM
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 87 TARGET_DEST_DEST
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 88 TARGET_DEST_DYNOBJ_NONE
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 89 TARGET_DEST_TRAJ
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 90 TARGET_UNIT_TARGET_MINIPET
        new StaticTargetData(TargetObjects.DEST, TargetReferences.DEST,   TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.RANDOM),      // 91 TARGET_DEST_DEST_RADIUS
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 92 TARGET_UNIT_SUMMONER
        new StaticTargetData(TargetObjects.CORPSE, TargetReferences.SRC,   TargetSelections.NYI,     TargetChecks.ENEMY,    TargetDirections.NONE),       // 93 TARGET_CORPSE_SRC_AREA_ENEMY
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 94 TARGET_UNIT_VEHICLE
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.PASSENGER, TargetDirections.NONE),       // 95 TARGET_UNIT_TARGET_PASSENGER
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 96 TARGET_UNIT_PASSENGER_0
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 97 TARGET_UNIT_PASSENGER_1
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 98 TARGET_UNIT_PASSENGER_2
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 99 TARGET_UNIT_PASSENGER_3
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 100 TARGET_UNIT_PASSENGER_4
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 101 TARGET_UNIT_PASSENGER_5
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 102 TARGET_UNIT_PASSENGER_6
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 103 TARGET_UNIT_PASSENGER_7
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.CONE,    TargetChecks.ENEMY,    TargetDirections.FRONT),       // 104 TARGET_UNIT_CONE_ENEMY_104
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 105 TARGET_UNIT_UNK_105
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.CHANNEL, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 106 TARGET_DEST_CHANNEL_CASTER
        new StaticTargetData(TargetObjects.NONE, TargetReferences.DEST,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 107 TARGET_UNK_DEST_AREA_UNK_107
        new StaticTargetData(TargetObjects.GOBJ, TargetReferences.CASTER, TargetSelections.CONE,    TargetChecks.DEFAULT,  TargetDirections.FRONT),       // 108 TARGET_GAMEOBJECT_CONE
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 109
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.CONE,    TargetChecks.DEFAULT,  TargetDirections.FRONT),       // 110 TARGET_DEST_UNK_110
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 111
        new StaticTargetData(TargetObjects.DEST, TargetReferences.CASTER, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 112
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 113
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 114
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 115
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 116
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 117
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.AREA,    TargetChecks.DEFAULT,  TargetDirections.NONE),        // 118
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.CASTER, TargetSelections.AREA,    TargetChecks.RAID,     TargetDirections.NONE),        // 119
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.NONE,   TargetSelections.AREA,    TargetChecks.DEFAULT,  TargetDirections.NONE),        // 120
        new StaticTargetData(TargetObjects.UNIT, TargetReferences.TARGET, TargetSelections.DEFAULT, TargetChecks.DEFAULT,  TargetDirections.NONE),        // 121
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 122
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 123
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 124
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 125
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 126
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 127
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 128
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 129
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 130
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 131
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 132
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 133
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 134
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 135
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 136
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 137
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 138
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 139
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 140
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 141
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 142
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 143
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 144
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 145
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 146
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 147
        new StaticTargetData(TargetObjects.NONE, TargetReferences.NONE,   TargetSelections.NYI,     TargetChecks.DEFAULT,  TargetDirections.NONE),        // 148
    };

    #endregion

    public static bool HasFlag(this TriggerCastFlags baseFlags, TriggerCastFlags flag)
    {
        return (baseFlags & flag) == flag;
    }
    public static bool HasFlag(this SpellPreventionType baseFlags, SpellPreventionType flag)
    {
        return (baseFlags & flag) == flag;
    }
    public static bool HasFlag(this SpellRangeFlag baseFlags, SpellRangeFlag flag)
    {
        return (baseFlags & flag) == flag;
    }
    public static bool HasFlag(this SpellCastTargetFlags baseFlags, SpellCastTargetFlags flag)
    {
        return (baseFlags & flag) == flag;
    }
    public static bool HasFlag(this TargetTypes baseFlags, TargetTypes flag)
    {
        return (baseFlags & flag) == flag;
    }

    public static TargetObjects ObjectType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].ObjectType;
    }
    public static TargetReferences ReferenceType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].ReferenceType;
    }
    public static TargetSelections Category(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].SelectionCategory;
    }
    public static TargetChecks CheckType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].SelectionCheckType;
    }
    public static TargetDirections DirectionType(this TargetTypes baseFlags)
    {
        return targetTypeData[(int)baseFlags].DirectionType;
    }
    public static bool IsArea(this TargetTypes baseFlags)
    {
        return baseFlags.Category() == TargetSelections.AREA || baseFlags.Category() == TargetSelections.CONE;
    }

}