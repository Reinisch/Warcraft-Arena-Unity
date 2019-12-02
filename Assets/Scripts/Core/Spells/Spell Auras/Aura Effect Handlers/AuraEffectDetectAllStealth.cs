namespace Core.AuraEffects
{
    public class AuraEffectDetectAllStealth : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectDetectAllStealth(Aura aura, AuraEffectInfoDetectAllStealth effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            auraApplication.Target.UpdateVisibility(true);
        }
    }
}