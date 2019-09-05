using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Spell Modifier", menuName = "Game Data/Spells/Auras/Effects/Spell Modifier", order = 2)]
    public class AuraEffectInfoSpellModifier : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly] private float modifierValue;
        [SerializeField, UsedImplicitly] private SpellModifierType modifierType;
        [SerializeField, UsedImplicitly] private SpellModifierApplicationType applicationType;

        public SpellModifierType ModifierType => modifierType;
        public SpellModifierApplicationType ApplicationType => applicationType;
        public override float Value => modifierValue;
        public override AuraEffectType AuraEffectType => AuraEffectType.SpeedIncreaseModifier;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSpellModifier(aura, this, index, Value);
        }
    }
}