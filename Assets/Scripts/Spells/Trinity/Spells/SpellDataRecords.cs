public class SpellChargeCategory
{
    public int Id { get; set; }
    public float ChargeRecoveryTime { get; set; }
    public int MaxCharges { get; set; }
    public int ChargeCategoryType { get; set; }
}

public class SpellCastTimes
{
    public int Id { get; set; }
    public float CastTime { get; set; }
    public float MinCastTime { get; set; }
    public float CastTimePerLevel { get; set; }
};

public class SpellDuration
{
    public int Id { get; set; }
    public int Duration { get; set; }
    public int MaxDuration { get; set; }
    public int DurationPerLevel { get; set; }
};

public class SpellRange
{
    public int Id { get; set; }
    public float MinRangeHostile { get; set; }
    public float MinRangeFriend { get; set; }
    public float MaxRangeHostile { get; set; }
    public float MaxRangeFriend { get; set; }
    public SpellRangeFlag Flags { get; set; }
};

public class SpellPowerCost
{
    public int Id { get; set; }

    public Powers PowerType { get; set; }
    public int ManaCost { get; set; }
    public float ManaCostPercentage { get; set; }
    public float HealthCostPercentage { get; set; }
    public float ManaCostPercentagePerSecond { get; set; }

    public int RequiredAura { get; set; }
};

public class SpellRadius
{
    public int Id { get; set; }
    public float Radius { get; set; }
    public float RadiusPerLevel { get; set; }
    public float RadiusMin { get; set; }
    public float RadiusMax { get; set; }
};

