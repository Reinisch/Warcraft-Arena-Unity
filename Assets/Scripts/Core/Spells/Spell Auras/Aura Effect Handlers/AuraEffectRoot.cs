namespace Core.AuraEffects
{
    public class AuraEffectRoot : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.RootStateChange;

        public AuraEffectRoot(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.UpdateControlState(UnitControlState.Root, apply);
        }
    }
}