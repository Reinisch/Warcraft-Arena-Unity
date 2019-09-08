using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Stun", menuName = "Game Data/Spells/Auras/Effects/Stun", order = 4)]
    public class AuraEffectInfoStun : AuraEffectInfo
    {
        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.StunState;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectStun(aura, this, index, Value);
        }
    }
}