namespace Core.AuraEffects
{
    public class AuraEffectStun : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Stun;

        public AuraEffectStun(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.UpdateControlState(UnitControlState.Stunned, apply);
        }
    }
}