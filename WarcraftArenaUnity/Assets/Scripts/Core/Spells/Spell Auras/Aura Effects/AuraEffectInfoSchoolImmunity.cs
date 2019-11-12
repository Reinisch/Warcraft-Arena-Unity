using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect School Immunity", menuName = "Game Data/Spells/Auras/Effects/School Immunity", order = 4)]
    public class AuraEffectInfoSchoolImmunity : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, EnumFlag] private SpellSchoolMask schoolMask;

        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.SchoolImmunity;
        public SpellSchoolMask SchoolMask => schoolMask;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSchoolImmunity(aura, this, index, Value);
        }
    }
}