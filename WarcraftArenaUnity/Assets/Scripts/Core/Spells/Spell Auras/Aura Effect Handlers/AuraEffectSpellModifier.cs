namespace Core.AuraEffects
{
    public class AuraEffectSpellModifier : AuraEffect
    {
        public new AuraEffectInfoSpellModifier EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        internal SpellModifier SpellModifier { get; }

        public AuraEffectSpellModifier(Aura aura, AuraEffectInfoSpellModifier effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
            SpellModifier = new SpellModifier(aura, effectInfo);
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.Spells.HandleSpellModifier(SpellModifier, apply);

            if (apply && mode == AuraEffectHandleMode.Refresh)
                CalculateValue();
        }

        internal override void CalculateValue()
        {
            SpellModifier.Value = Value = BaseValue = EffectInfo.Value;
        }
    }
}