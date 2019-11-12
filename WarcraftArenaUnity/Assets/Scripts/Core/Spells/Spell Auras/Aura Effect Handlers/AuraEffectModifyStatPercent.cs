namespace Core.AuraEffects
{
    public class AuraEffectModifyStatPercent : AuraEffect
    {
        public new AuraEffectInfoModifyStatPercent EffectInfo { get; }

        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectModifyStatPercent(Aura aura, AuraEffectInfoModifyStatPercent effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.Attributes.HandleStatPercentModifier(EffectInfo.StatType, StatModifierType.BasePercent, Value, apply);
        }
    }
}