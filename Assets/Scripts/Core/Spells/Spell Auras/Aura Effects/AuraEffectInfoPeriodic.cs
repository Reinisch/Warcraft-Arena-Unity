using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    public abstract class AuraEffectInfoPeriodic : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(0, 30000)] private int period;
        [SerializeField, UsedImplicitly] private bool startPeriodicOnApply;

        public int Period => period;
        public bool StartPeriodicOnApply => startPeriodicOnApply;
    }
}