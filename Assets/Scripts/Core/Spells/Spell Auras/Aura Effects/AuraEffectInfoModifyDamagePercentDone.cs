using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Damage Percent Done", menuName = "Game Data/Spells/Auras/Effects/Modify Damage Percent Done", order = 3)]
    public class AuraEffectInfoModifyDamagePercentDone : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-99f, 500f)] private float damagePercentDone;

        public override float Value => damagePercentDone;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyDamagePercentDone;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSharedBasicModifer(aura, this, index, Value);
        }
    }
}