using System;

namespace Core
{
    public struct GameEntityTargetInfo
    {
        public Guid TargetGUID;
        public long TimeDelay;
        public int EffectMask;
        public bool Processed;
    }
}