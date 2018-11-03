namespace Core
{
    public static class GlobalHelper
    {
        public const Expansions CurrentExpansion = Expansions.Legion;
        public const int MaxExpansions = 8;


        public const int MaxHolidayDurations = 10;
        public const int MaxHolidayDates = 16;
        public const int MaxHolidayFlags = 10;


        public const int MaxItemExtCostItems = 5;
        public const int MaxItemExtCostCurrencies = 5;
        public const int MaxItemRandomProperties = 5;
        public const int MaxItemSetItems = 17;
        public const int MaxItemProtoFlags = 3;
        public const int MaxItemProtoSockets = 3;
        public const int MaxItemProtoStats = 10;
        public const int MaxItemQuality = 8;

        public const int MaxPowersPerClass = 6;
        public const int MaxRaces = 27;
        public const int RacemaskNeutral = 1 << ((int) Races.PandarenNeutral - 1);
        public const int RacemaskHorde = RaceMaskAllPlayable & ~RacemaskAlliance;

        public const int RacemaskAlliance = 
            (1 << ((int) Races.Human - 1)) |
            (1 << ((int) Races.Dwarf - 1)) |
            (1 << ((int) Races.Nightelf - 1)) |
            (1 << ((int) Races.Gnome - 1)) |
            (1 << ((int) Races.Draenei - 1)) |
            (1 << ((int) Races.Worgen - 1)) |
            (1 << ((int) Races.PandarenAlliance - 1));

        public const int RaceMaskAllPlayable = 
            (1 << ((int) Races.Human - 1)) |
            (1 << ((int) Races.Orc - 1)) |
            (1 << ((int) Races.Dwarf - 1)) |
            (1 << ((int) Races.Nightelf - 1)) |
            (1 << ((int) Races.UndeadPlayer - 1)) |
            (1 << ((int) Races.Tauren - 1)) |
            (1 << ((int) Races.Troll - 1)) |
            (1 << ((int)Races.Gnome - 1)) |
            (1 << ((int) Races.Bloodelf - 1)) |
            (1 << ((int) Races.Draenei - 1)) |
            (1 << ((int) Races.Goblin - 1)) |
            (1 << ((int) Races.Worgen - 1)) |
            (1 << ((int) Races.PandarenHorde - 1)) |
            (1 << ((int) Races.PandarenAlliance - 1)) |
            (1 << ((int) Races.PandarenNeutral - 1));

        public const int ClassmaskAllCreatures = 
            (1 << ((int) UnitClass.Warrior - 1)) |
            (1 << ((int) UnitClass.Paladin - 1)) |
            (1 << ((int) UnitClass.Rogue - 1)) |
            (1 << ((int) UnitClass.Mage - 1));

        public const int ClassmaskWandUsers = 
            (1 << ((int) Classes.Priest - 1)) |
            (1 << ((int) Classes.Mage - 1)) |
            (1 << ((int) Classes.Warlock - 1));

        public const int ClassmaskAllPlayable =
            (1 << ((int) Classes.Warrior - 1)) |
            (1 << ((int) Classes.Paladin - 1)) |
            (1 << ((int) Classes.Hunter - 1)) |
            (1 << ((int) Classes.Rogue - 1)) |
            (1 << ((int) Classes.Priest - 1)) |
            (1 << ((int) Classes.DeathKnight - 1)) |
            (1 << ((int) Classes.Shaman - 1)) |
            (1 << ((int) UnitClass.Mage - 1)) |
            (1 << ((int) Classes.Warlock - 1)) |
            (1 << ((int) Classes.Monk - 1)) |
            (1 << ((int) Classes.Druid - 1)) |
            (1 << ((int) Classes.DemonHunter - 1));

        public const ReputationRank MinReputationRank = ReputationRank.Hated;
        public const int MaxReputationRank = 8;
        public const int MaxSpilloverFactions = 5;
        public const int PlayerMaxBattlegroundQueues = 2;

        public const int MaxConditionTargets = 3;
        public const int CriteriaTypeTotal = 190;
        public const int TaxiMaskSize = 239;
    }
}