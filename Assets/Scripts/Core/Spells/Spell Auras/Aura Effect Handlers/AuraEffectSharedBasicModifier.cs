namespace Core.AuraEffects
{
    public class AuraEffectSharedBasicModifer : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.BasicModifier;

        public AuraEffectSharedBasicModifer(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }
    }
}