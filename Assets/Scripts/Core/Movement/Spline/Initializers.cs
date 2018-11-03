namespace Core
{
    public interface ISplineInitializer
    {
        int ComputeSplineTime(Spline spline, int i);
    }

    public struct FallInitializer : ISplineInitializer
    {
        private readonly float startElevation;


        public FallInitializer(float startElevation)
        {
            this.startElevation = startElevation;
        }

        public int ComputeSplineTime(Spline s, int i)
        {
            return (int)MovementHelper.ComputeFallTime(startElevation - s.GetPoint(i+1).z, false) * 1000;
        }
    }

    public struct CommonInitializer : ISplineInitializer
    {
        private readonly float velocityInv;
        private int time;


        public CommonInitializer(float velocity)
        {
            velocityInv = 1000.0f / velocity;
            time = 1;
        }
    
        public int ComputeSplineTime(Spline s, int i)
        {
            time += (int)(s.SegLength(i) * velocityInv);
            return time;
        }
    }
}