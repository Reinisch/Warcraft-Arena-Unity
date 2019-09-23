using Common;
using JetBrains.Annotations;

namespace Core.AuraEffects
{
    public abstract class AuraEffectPeriodic : AuraEffect
    {
        private int periodicTimeLeft;
        private int period;
        private int tickNumber;

        public new AuraEffectInfoPeriodic EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.PeriodicDamage;

        protected AuraEffectPeriodic(Aura aura, AuraEffectInfoPeriodic effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (apply)
                PreparePeriodic(Aura.Caster, mode == AuraEffectHandleMode.Refresh);
        }

        public override void DoUpdate(int deltaTime)
        {
            base.DoUpdate(deltaTime);

            if (Aura.Duration >= 0)
            {
                if (periodicTimeLeft > deltaTime)
                    periodicTimeLeft -= deltaTime;
                else
                {
                    tickNumber++;
                    periodicTimeLeft += period - deltaTime;

                    HandleTickOnApplications();
                }
            }
        }

        protected abstract void HandlePeriodic(Unit target, [CanBeNull] Unit caster);

        private void HandleTickOnApplications()
        {
            for (int i = Aura.Applications.Count - 1; i >= 0; i--)
            {
                AuraApplication application = Aura.Applications[i];
                if (application.AppliedEffectMask.HasBit(Index))
                    HandlePeriodic(application.Target, Aura.Caster);
            }
        }

        private void PreparePeriodic(Unit caster, bool refreshed)
        {
            period = EffectInfo.Period;

            if (period > 0 && caster != null && Aura.AuraInfo.HasAttribute(AuraAttributes.HasteAffectsDuration))
                period = (int)(period * caster.ModHaste);

            if (refreshed)
            {
                if(EffectInfo.StartPeriodicOnApply)
                    periodicTimeLeft = 0;
            }
            else
            {
                tickNumber = 0;
                periodicTimeLeft = EffectInfo.StartPeriodicOnApply ? 0 : period;
            }
        }
    }
}