using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Stat Percent", menuName = "Game Data/Spells/Auras/Effects/Modify Stat Percent", order = 3)]
    public class AuraEffectInfoModifyStatPercent : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-99.99f, 500f)]
        private float statPercent;
        [SerializeField, UsedImplicitly]
        private StatType statType;

        public override float Value => statPercent;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyStatPercent;
        public StatType StatType => statType;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectModifyStatPercent(aura, this, index, Value);
        }
    }
}