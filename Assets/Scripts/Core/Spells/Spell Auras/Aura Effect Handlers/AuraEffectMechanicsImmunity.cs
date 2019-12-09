using System.Collections.Generic;

namespace Core.AuraEffects
{
    public class AuraEffectMechanicsImmunity : AuraEffect
    {
        public new AuraEffectInfoMechanicsImmunity EffectInfo { get; }

        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectMechanicsImmunity(Aura aura, AuraEffectInfoMechanicsImmunity effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            SpellMechanicsFlags immuneMechanics = 0;
            for (int i = 0; i < EffectInfo.ImmuneMechanics.Count; i++)
            {
                auraApplication.Target.Spells.ModifyMechanicsImmunity(Aura.SpellInfo, EffectInfo.ImmuneMechanics[i], apply);
                immuneMechanics |= EffectInfo.ImmuneMechanics[i].AsFlag();
            }

            if (apply && Aura.SpellInfo.HasAttribute(SpellAttributes.DispelAurasOnImmunity))
            {
                var applicaionsToRemove = new List<AuraApplication>();
                for (var index = 0; index < auraApplication.Target.AuraApplications.Count; index++)
                {
                    AuraApplication otherApplication = auraApplication.Target.AuraApplications[index];
                    if (!otherApplication.Aura.AuraInfo.HasAnyMechanics(immuneMechanics))
                        continue;

                    if (!Aura.CanDispel(otherApplication.Aura))
                        continue;

                    applicaionsToRemove.Add(otherApplication);
                }

                foreach (AuraApplication applicationToRemove in applicaionsToRemove)
                    if (applicationToRemove.RemoveMode == AuraRemoveMode.None)
                        auraApplication.Target.Auras.RemoveAuraWithApplication(applicationToRemove, AuraRemoveMode.Immunity);
            }
        }
    }
}