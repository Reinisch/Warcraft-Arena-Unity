using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Confuse", menuName = "Game Data/Spells/Auras/Effects/Confuse", order = 4)]
    public class AuraEffectInfoConfuse : AuraEffectInfo
    {
        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.ConfusionState;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectConfuse(aura, this, index, Value);
        }
    }
}