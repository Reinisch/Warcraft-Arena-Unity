public class SpellRange
{
    public int Id { get; set; }
    public float MinRangeHostile { get; set; }
    public float MinRangeFriend { get; set; }
    public float MaxRangeHostile { get; set; }
    public float MaxRangeFriend { get; set; }
    public SpellRangeFlag Flags { get; set; }
}