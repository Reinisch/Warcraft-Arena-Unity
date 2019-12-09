namespace Core.AuraEffects
{
    public class AuraEffectModifyMaxPower : AuraEffect
    {
        private readonly UnitModifierType modifierType;

        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectModifyMaxPower(Aura aura, AuraEffectInfoModifyMaxPower effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            modifierType = effectInfo.PowerType.AsModifier();
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.Attributes.HandleStatPercentModifier(modifierType, StatModifierType.BaseValue, Value, apply);
        }
    }
}