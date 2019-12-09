using Common;

namespace Core.AuraEffects
{
    public class AuraEffectSpellTrigger : AuraEffect
    {
        public new AuraEffectInfoSpellTrigger EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectSpellTrigger(Aura aura, AuraEffectInfoSpellTrigger effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode == AuraEffectHandleMode.Normal)
                auraApplication.Target.Spells.HandleSpellTrigger(this, apply);
        }

        public bool WillTrigger(SpellTriggerActivationInfo activationInfo)
        {
            if (activationInfo.Spell.IsTriggered)
                return false;

            if (!EffectInfo.CanCasterBeTriggerTarget && activationInfo.Actor == activationInfo.ActionTarget)
                return false;

            if (Aura.AuraInfo.UsesCharges && Aura.Charges < 1)
                return false;

            if (!activationInfo.Actor.SpellHistory.IsReady(EffectInfo.TriggeredSpell))
                return false;

            if (!EffectInfo.TriggerFlags.HasAnyFlag(activationInfo.SpellTriggerFlags))
                return false;

            for (int i = 0; i < EffectInfo.TriggerConditions.Count; i++)
                if (EffectInfo.TriggerConditions[i].IsApplicableAndInvalid(activationInfo.Actor, activationInfo.ActionTarget, activationInfo.Spell))
                    return false;

            return RandomUtils.CheckSuccess(EffectInfo.Chance + activationInfo.Spell.ConsumedComboPoints * EffectInfo.ChancePerCombo);
        }
    }
}