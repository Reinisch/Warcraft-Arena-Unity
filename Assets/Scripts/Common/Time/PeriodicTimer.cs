namespace Common
{
    public class PeriodicTimer
    {
        public int Period { get; private set; }
        public int ExpireTime { get; private set; }
        public bool Passed => ExpireTime <= 0;


        public PeriodicTimer(int period, int startTime)
        {
            Period = period;
            ExpireTime = startTime;
        }

        public void SetPeriodic(int period, int startTime)
        {
            Period = period;
            ExpireTime = startTime;
        }

        public bool Update(int diff)
        {
            if ((ExpireTime -= diff) > 0)
                return false;

            ExpireTime += Period > diff ? Period : diff;
            return true;
        }

        public void TrackerUpdate(int diff)
        {
            ExpireTime -= diff;
        }

        public void TrackerReset(int diff, int period)
        {
            ExpireTime += period > diff ? period : diff;
        }
    }
}