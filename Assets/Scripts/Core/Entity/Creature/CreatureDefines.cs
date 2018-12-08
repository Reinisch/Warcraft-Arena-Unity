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
        TimedOrDeadDespawn = 1,
        TimedOrCorpseDespawn = 2,
        TimedDespawn = 3,
        TimedDespawnOutOfCombat = 4,
        CorpseDespawn = 5,
        CorpseTimedDespawn = 6,
        DeadDespawn = 7,
        ManualDespawn = 8
    }

    [Flags]
    public enum InhabitTypeValues
    {
        Ground = 1,
        Water = 2,
        Air = 4,
        Anywhere = Ground | Water | Air
    }
}