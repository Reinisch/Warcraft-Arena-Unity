namespace Common
{
    public struct TimeTracker
    {
        public long ExpiryTime { get; private set; }
        public bool Passed => ExpiryTime <= 0;

        public TimeTracker(long expiry = 0)
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