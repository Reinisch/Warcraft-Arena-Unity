using System.Collections.Generic;
using Common;
using JetBrains.Annotations;

namespace Core
{
    public class AuraApplication
    {
        private static int ApplicationAliveCount;

        [NotNull]
        public Aura Aura { get; }
        [NotNull]
        public Unit Target { get; }

        public int EffectsToApply { get; }
        public int AppliedEffectMask { get; private set; }
        public AuraRemoveMode RemoveMode { get; internal set; }

        public AuraApplication(Unit target, Aura aura, int auraEffectMask)
        {
            Target = target;
            Aura = aura;

            EffectsToApply = auraEffectMask;

            Logging.LogAura($"Created new application for target: {target.Name} for aura: {Aura.AuraInfo.name}, current count: {++ApplicationAliveCount}");
        }

        ~AuraApplication()
        {
            Logging.LogAura($"Finalized application, current count: {--ApplicationAliveCount}");
        }

        internal void HandleEffect(int auraEffectIndex, bool apply, HashSet<AuraEffectHandleGroup> handleGroups)
        {
            AppliedEffectMask = AppliedEffectMask.SetBit(auraEffectIndex, apply);
            AuraEffect effectToHandle = Aura.Effects[auraEffectIndex];

            if (!handleGroups.Contains(effectToHandle.HandleGroup))
            {
                Aura.Effects[auraEffectIndex].HandleEffect(this, AuraEffectHandleMode.Normal, apply);

                if (effectToHandle.HandleGroup != AuraEffectHandleGroup.Unique)
                    handleGroups.Add(effectToHandle.HandleGroup);
            }
        }
    }
}