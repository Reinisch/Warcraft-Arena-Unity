using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Root State", menuName = "Game Data/Spells/Auras/Effects/State Root", order = 4)]
    public class AuraEffectInfoRoot : AuraEffectInfo
    {
        public override float Value => 1.0f;
        public override AuraEffectType AuraEffectType => AuraEffectType.RootState;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectRoot(aura, this, index, Value);
        }
    }
}