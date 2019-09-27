using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Ignore Target Aura State", menuName = "Game Data/Spells/Auras/Effects/Ignore Target Aura State", order = 4)]
    public class AuraEffectInfoIgnoreTargetAuraState : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly] private AuraStateType ignoredState;

        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.IgnoreTargetAuraState;
        public AuraStateType IgnoredState => ignoredState;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectDummy(aura, this, index, Value);
        }
    }
}