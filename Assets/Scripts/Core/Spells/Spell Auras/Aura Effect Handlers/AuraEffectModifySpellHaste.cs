namespace Core.AuraEffects
{
    public class AuraEffectModifySpellHaste : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectModifySpellHaste(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.Attributes.ModHaste.ModifyPercentage(Value, apply);
        }
    }
}