namespace Core.AuraEffects
{
    public class AuraEffectSlowFall : AuraEffect
    {
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectSlowFall(Aura aura, AuraEffectInfoSlowFall effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            int slowFallSpeed = (int)auraApplication.Target.Auras.MinPositiveAuraModifier(AuraEffectType.SlowFall);
            auraApplication.Target.SlowFallSpeed = slowFallSpeed;
        }
    }
}