using System.Collections.Generic;

namespace Core.AuraEffects
{
    public class AuraEffectSchoolImmunity : AuraEffect
    {
        public new AuraEffectInfoSchoolImmunity EffectInfo { get; }

        public override AuraEffectHandleGroup HandleGroup => AuraEffectHandleGroup.Unique;

        public AuraEffectSchoolImmunity(Aura aura, AuraEffectInfoSchoolImmunity effectInfo, int index, float value) : base(aura, effectInfo, index, value)
        {
            EffectInfo = effectInfo;
        }

        public override void HandleEffect(AuraApplication auraApplication, AuraEffectHandleMode mode, bool apply)
        {
            base.HandleEffect(auraApplication, mode, apply);

            if (mode != AuraEffectHandleMode.Normal)
                return;

            auraApplication.Target.Spells.ModifySchoolImmunity(Aura.SpellInfo, EffectInfo.SchoolMask, apply);

            if (apply && Aura.SpellInfo.HasAttribute(SpellAttributes.DispelAurasOnImmunity))
            {
                var applicaionsToRemove = new List<AuraApplication>();
                for (var index = 0; index < auraApplication.Target.AuraApplications.Count; index++)
                {
                    AuraApplication otherApplication = auraApplication.Target.AuraApplications[index];
                    if (!otherApplication.Aura.SpellInfo.SchoolMask.HasAnyFlag(EffectInfo.SchoolMask))
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