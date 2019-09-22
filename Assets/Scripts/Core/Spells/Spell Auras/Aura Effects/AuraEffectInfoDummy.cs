using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Dummy", menuName = "Game Data/Spells/Auras/Effects/Dummy", order = 2)]
    public class AuraEffectInfoDummy : AuraEffectInfo
    {
        public override float Value => 0.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.Dummy;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectDummy(aura, this, index, Value);
        }
    }
}