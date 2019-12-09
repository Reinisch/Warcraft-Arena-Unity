namespace Core.AuraEffects
{
    public class AuraEffectModifyStealthDetection : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectModifyStealthDetection(Aura aura, AuraEffectInfoModifyStealthDetection effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            if (apply)
                auraApplication.Target.StealthDetection += (int)EffectInfo.Value;
            else
                auraApplication.Target.StealthDetection -= (int)EffectInfo.Value;

            auraApplication.Target.UpdateVisibility(true);
        }
    }
}