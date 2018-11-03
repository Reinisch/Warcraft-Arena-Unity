namespace Core
{
    public class DiminishingReturn
    {
        public DiminishingGroup DrGroup { get; private set; }
        public ushort Stack { get; private set; }
        public uint HitTime { get; private set; }
        public uint HitCount { get; private set; }

        public DiminishingReturn(DiminishingGroup group, uint t, uint count)
        {
            DrGroup = DiminishingGroup.None;
            HitTime = t;
            HitCount = count;
        }
    }
}