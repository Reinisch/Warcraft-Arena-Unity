namespace Client
{
    public static class EnumUtils
    {
        public static bool HasTargetFlag(this TargetingDeathState baseFlags, TargetingDeathState flag)
        {
            return (baseFlags & flag) == flag;
        }
    }
}
