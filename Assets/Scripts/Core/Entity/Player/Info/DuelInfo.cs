namespace Core
{
    public class DuelInfo
    {
        public Player Initiator { get; set; }
        public Player Opponent { get; set; }
        public long StartTimer { get; set; }
        public long StartTime { get; set; }
        public long OutOfBound { get; set; }
        public bool IsMounted { get; set; }

        public DuelInfo()
        {
            Initiator = null;
            Opponent = null;
            StartTimer = 0;
            StartTime = 0;
            OutOfBound = 0;
            IsMounted = false;
        }
    }
}