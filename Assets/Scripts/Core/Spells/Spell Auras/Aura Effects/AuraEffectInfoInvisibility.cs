using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Invisibility", menuName = "Game Data/Spells/Auras/Effects/Invisibility", order = 3)]
    public class AuraEffectInfoInvisibility : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(0.0f, 100f)]
        private int invisibilityPower;

        public override float Value => invisibilityPower;
        public override AuraEffectType AuraEffectType => AuraEffectType.Invisibility;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectInvisiblity(aura, this, index, Value);
        }
    }
}