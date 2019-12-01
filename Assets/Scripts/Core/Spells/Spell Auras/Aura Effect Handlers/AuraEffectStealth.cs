namespace Core.AuraEffects
{
    public class AuraEffectStealth : AuraEffect
    {
        public new AuraEffectInfoStealth EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectStealth(Aura aura, AuraEffectInfoStealth effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            if (apply)
                auraApplication.Target.StealthSubtlety += (int)EffectInfo.Value;
            else
                auraApplication.Target.StealthSubtlety -= (int)EffectInfo.Value;

            auraApplication.Target.UpdateVisibility(true);
        }
    }
}