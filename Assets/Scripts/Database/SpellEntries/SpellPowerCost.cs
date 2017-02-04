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