namespace Core.AuraEffects
{
    public class AuraEffectModifyInvisibilityDetection : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectModifyInvisibilityDetection(Aura aura, AuraEffectInfoModifyInvisibilityDetection effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            if (apply)
                auraApplication.Target.InvisibilityDetection += (int)EffectInfo.Value;
            else
                auraApplication.Target.InvisibilityDetection -= (int)EffectInfo.Value;

            auraApplication.Target.UpdateVisibility(true);
        }
    }
}