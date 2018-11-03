namespace Core
{
    public class BarberChairEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.BarberChair;

        public uint Chairheight { get; set; }                             // 0 chairheight, int, Min value: 0, Max value: 2, Default value: 1
        public int HeightOffset { get; set; }                             // 1 Height Offset (inches), int, Min value: -100, Max value: 100, Default value: 0
        public uint SitAnimKit { get; set; }                              // 2 Sit Anim Kit, References: AnimKit, NoValue = 0
    }
}