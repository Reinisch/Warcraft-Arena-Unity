using System.Collections.Generic;
using Core.Conditions;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Override Spell Crit Calculation", menuName = "Game Data/Spells/Auras/Effects/Override Spell Crit Calculation", order = 3)]
    public class AuraEffectInfoOverrideSpellCritCalculation : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(0.0f, 5f)]
        private float multiplier;
        [SerializeField, UsedImplicitly, Range(-100.0f, 100f)]
        private float modifier;
        [SerializeField, UsedImplicitly]
        private List<Condition> validTargetConditions;

        public override float Value => multiplier;
        public override AuraEffectType AuraEffectType => AuraEffectType.OverrideSpellCritCalculation;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSharedBasicModifer(aura, this, index, Value);
        }

        public void ModifySpellCrit(Unit caster, Unit target, SpellInfo spellInfo, ref float currentCritChance, Spell spell = null)
        {
            foreach (var condition in validTargetConditions)
                if (condition.IsApplicableAndInvalid(caster, target, spell, spellInfo))
                    return;

            currentCritChance *= multiplier;
            currentCritChance += modifier;
        }
    }
}