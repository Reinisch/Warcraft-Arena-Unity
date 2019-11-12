namespace Core.AuraEffects
{
    public class AuraEffectPreventCasting : AuraEffect
    {
        public new AuraEffectInfoPreventCasting EffectInfo { get; }
        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectPreventCasting(Aura aura, AuraEffectInfoPreventCasting effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            Unit target = auraApplication.Target;
            bool hasSilence = EffectInfo.PreventionType.HasTargetFlag(SpellPreventionType.Silence);
            bool hasPacify = EffectInfo.PreventionType.HasTargetFlag(SpellPreventionType.Pacify);

            if (apply)
            {
                if (hasSilence)
                    target.SetFlag(UnitFlags.Silenced);

                if (hasPacify)
                    target.SetFlag(UnitFlags.Pacified);

                if (target.SpellCast.IsCasting && target.SpellCast.Spell.SpellInfo.PreventionType.HasAnyFlag(EffectInfo.PreventionType))
                    target.SpellCast.Cancel();
            }
            else
            {
                if (hasSilence && !target.HasAuraType(AuraEffectType.Silence) && !target.HasAuraType(AuraEffectType.SilencePacify))
                    target.RemoveFlag(UnitFlags.Silenced);

                if (hasPacify && !target.HasAuraType(AuraEffectType.Pacify) && !target.HasAuraType(AuraEffectType.SilencePacify))
                    target.RemoveFlag(UnitFlags.Pacified);
            }
        }
    }
}