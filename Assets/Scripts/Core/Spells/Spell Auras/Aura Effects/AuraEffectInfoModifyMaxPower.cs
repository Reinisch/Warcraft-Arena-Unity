using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Max Power", menuName = "Game Data/Spells/Auras/Effects/Modify Max Power", order = 3)]
    public class AuraEffectInfoModifyMaxPower : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly]
        private int power;
        [SerializeField, UsedImplicitly]
        private SpellPowerType powerType;

        public override float Value => power;
        public override AuraEffectType AuraEffectType => AuraEffectType.ModifyMaxPower;
        public SpellPowerType PowerType => powerType;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectModifyMaxPower(aura, this, index, Value);
        }
    }
}