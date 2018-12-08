namespace Game
{
    public class ConnectionAttemptInfo
    {
        public float TimeSinceAttempt { get; set; }
        public bool IsConnected { get; set; }
        public bool IsFailed { get; set; }
        public bool IsRefused { get; set; }

        public void Reset()
        {
            TimeSinceAttempt = 0.0f;
            IsConnected = false;
            IsFailed = false;
            IsRefused = false;
        }
    }
}