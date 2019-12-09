namespace Core.AuraEffects
{
    public class AuraEffectModifyStatPercent : AuraEffect
    {
        private readonly UnitModifierType modifierType;

        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectModifyStatPercent(Aura aura, AuraEffectInfoModifyStatPercent effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            modifierType = effectInfo.StatType.AsModifier();
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.Attributes.HandleStatPercentModifier(modifierType, StatModifierType.BasePercent, Value, apply);
        }
    }
}