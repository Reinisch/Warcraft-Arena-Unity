namespace Core
{
    public class PlayerLevelInfo
    {
        public ushort[] Stats { get; set; }

        public PlayerLevelInfo()
        {
            Stats = new ushort[StatHelper.MaxStats];
        }
    }
}