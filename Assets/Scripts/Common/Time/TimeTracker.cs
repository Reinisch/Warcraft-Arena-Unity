namespace Common
{
    public class TimeTracker
    {
        public long ExpiryTime { get; private set; }
        public bool Passed => ExpiryTime <= 0;


        public TimeTracker(long expiry)
        {
            ExpiryTime = expiry;
        }

        public void Update(long diff)
        {
            ExpiryTime -= diff;
        }

        public void Reset(long interval)
        {
            ExpiryTime = interval;
        }
    }
}