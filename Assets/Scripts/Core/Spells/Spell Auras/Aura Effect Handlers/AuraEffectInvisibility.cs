namespace Core.AuraEffects
{
    public class AuraEffectInvisiblity : AuraEffect
    {
        public new AuraEffectInfoInvisibility EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectInvisiblity(Aura aura, AuraEffectInfoInvisibility effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            if (apply)
                auraApplication.Target.InvisibilityPower += (int)EffectInfo.Value;
            else
                auraApplication.Target.InvisibilityPower -= (int)EffectInfo.Value;

            bool hasInvisibilityAura = auraApplication.Target.Auras.HasAuraType(AuraEffectType.Invisibility);
            UnitVisualEffectFlags visualEffectFlags = auraApplication.Target.Attributes.VisualEffectFlags;
            auraApplication.Target.Attributes.VisualEffectFlags = visualEffectFlags.SetFlag(UnitVisualEffectFlags.InvisibilityTransparency, hasInvisibilityAura);
            auraApplication.Target.UpdateVisibility(true);
        }
    }
}