using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Modify Spell Haste", menuName = "Game Data/Spells/Auras/Effects/Spell Haste Modifier", order = 2)]
    public class AuraEffectInfoModifySpellHaste : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(-500f, 500.0f)] private float castTimePercentageModifier;

        public override float Value => castTimePercentageModifier;
        public override AuraEffectType AuraEffectType => AuraEffectType.HasteSpells;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectModifySpellHaste(aura, this, index, Value);
        }
    }
}