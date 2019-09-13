using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Damage Percent Taken", menuName = "Game Data/Spells/Auras/Effects/Modify Damage Percent Taken", order = 3)]
    public class AuraEffectInfoModifyDamagePercentTaken : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-100f, 500f)] private float damagePercentTaken;

        public override float Value => damagePercentTaken;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyDamagePercentTaken;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSharedBasicModifer(aura, this, index, Value);
        }
    }
}