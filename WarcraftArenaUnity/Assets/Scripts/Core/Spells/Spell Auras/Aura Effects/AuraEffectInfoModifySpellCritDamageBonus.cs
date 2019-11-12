using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Crit Damage Bonus", menuName = "Game Data/Spells/Auras/Effects/Modify Crit Damage Bonus", order = 3)]
    public class AuraEffectInfoModifySpellCritDamageBonus : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-100f, 500f)] private float critDamagePercentageMultiplier;

        public override float Value => critDamagePercentageMultiplier;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyCritDamageBonus;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSharedBasicModifer(aura, this, index, Value);
        }
    }
}