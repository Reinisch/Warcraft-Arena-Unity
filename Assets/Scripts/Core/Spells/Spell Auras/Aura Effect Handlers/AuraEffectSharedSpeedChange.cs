namespace Core.AuraEffects
{
    public class AuraEffectSharedSpeedChange : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.SpeedChange;

        public AuraEffectSharedSpeedChange(Aura aura, AuraEffectInfo effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.Attributes.UpdateSpeed(UnitMoveType.Run);
        }
    }
}