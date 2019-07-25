namespace Client
{
    internal enum EffectState
    {
        Unused,
        Idle,
        Active,
        Fading
    }

    internal static class EffectStateUtils
    {
        public static bool IsPlaying(this EffectState state) => state == EffectState.Active || state == EffectState.Fading;
        public static bool IsIdle(this EffectState state) => state == EffectState.Idle;
    }
}
