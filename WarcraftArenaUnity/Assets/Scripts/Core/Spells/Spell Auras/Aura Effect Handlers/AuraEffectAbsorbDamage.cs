namespace Core.AuraEffects
{
    public class AuraEffectAbsorbDamage : AuraEffect
    {
        public new AuraEffectInfoAbsorbDamage EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectAbsorbDamage(Aura aura, AuraEffectInfoAbsorbDamage effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (apply && mode == AuraEffectHandleMode.Refresh)
                CalculateValue();
        }

        internal override void CalculateValue()
        {
            Value = BaseValue = EffectInfo.CalculateAbsorbAmount(Aura.Caster);
        }
    }
}