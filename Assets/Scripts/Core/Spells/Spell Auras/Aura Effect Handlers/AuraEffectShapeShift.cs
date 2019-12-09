namespace Core.AuraEffects
{
    public class AuraEffectShapeShift : AuraEffect
    {
        public new AuraEffectInfoShapeShift EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectShapeShift(Aura aura, AuraEffectInfoShapeShift effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal && mode != AuraEffectHandleMode.Refresh)
                return;

            if (apply)
            {
                auraApplication.Target.Auras.RemoveAurasWithEffect(AuraEffectType.ShapeShift, this);

                auraApplication.Target.UpdateShapeShiftForm(this);
            }
            else
            {
                auraApplication.Target.ResetShapeShiftForm();
            }

            auraApplication.Target.Attributes.UpdateDisplayPower();
        }
    }
}