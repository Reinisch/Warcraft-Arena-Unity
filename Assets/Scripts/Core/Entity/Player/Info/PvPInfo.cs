namespace Core
{
    public class PvPInfo
    {
        public bool IsHostile { get; set; }
        public bool IsInHostileArea { get; set; }           // Marks if player is in an area which forces PvP flag
        public bool IsInNoPvPArea { get; set; }             // Marks if player is in a sanctuary or friendly capital city
        public bool IsInFFAArea { get; set; }               // Marks if player is in an FFAPvP area (such as Gurubashi Arena)
        public long EndTimer { get; set; }                  // Time when player unflags himself for PvP (flag removed after 5 minutes)


        public PvPInfo()
        {
            IsHostile = false;
            IsInHostileArea = false;
            IsInNoPvPArea = false;
            IsInFFAArea = false;
            EndTimer = 0;
        }
    }
}