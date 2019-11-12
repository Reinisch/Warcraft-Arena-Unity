using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Suppress Movement Slow Effects", menuName = "Game Data/Spells/Auras/Effects/Suppress Movement Slow", order = 5)]
    public class AuraEffectInfoSuppressMovementSlowEffects : AuraEffectInfo
    {
        public override float Value => 0.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.SpeedSupressSlowEffects;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSharedSpeedChange(aura, this, index, Value);
        }
    }
}