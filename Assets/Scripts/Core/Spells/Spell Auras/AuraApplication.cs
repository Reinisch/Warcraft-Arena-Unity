namespace Core
{
    public class AuraApplication
    {
        private AuraRemoveMode removeMode;

        public Unit Target { get; }
        public Unit Caster { get; }
        public Aura Aura { get; }

        public bool IsRemoved => removeMode != AuraRemoveMode.None;

        public int EffectsToApply { get; }
        public int AppliedEffectMask { get; private set; }

        public AuraApplication(Unit target, Unit caster, Aura aura, int auraEffectMask)
        {
            Target = target;
            Caster = caster;
            Aura = aura;

            EffectsToApply = auraEffectMask;
        }

        internal void Remove(AuraRemoveMode removeMode)
        {
            this.removeMode = removeMode;
        }

        internal void HandleEffect(int auraEffectIndex, bool apply)
        {
            AppliedEffectMask = AppliedEffectMask.SetBit(auraEffectIndex, apply);
            Aura.Effects[auraEffectIndex].HandleEffect(this, AuraEffectHandleMode.Normal, apply);
        }
    }
}