namespace Core.AuraEffects
{
    public class AuraEffectConfuse : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Confuse;

        public AuraEffectConfuse(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.UpdateControlState(UnitControlState.Confused, apply);
        }
    }
}