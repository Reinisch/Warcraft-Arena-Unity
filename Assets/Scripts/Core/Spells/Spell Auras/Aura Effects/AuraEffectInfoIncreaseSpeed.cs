using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Increase Speed", menuName = "Game Data/Spells/Auras/Effects/Speed Increase", order = 2)]
    public class AuraEffectInfoIncreaseSpeed : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(1.0f, 500.0f)] private float increasePercent;

        public override float Value => increasePercent;
        public override AuraEffectType AuraEffectType => AuraEffectType.SpeedIncreaseModifier;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSharedSpeedChange(aura, this, index, Value);
        }
    }
}