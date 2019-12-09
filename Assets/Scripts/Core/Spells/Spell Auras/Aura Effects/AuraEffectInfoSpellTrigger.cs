using System.Collections.Generic;
using Core.Conditions;
using JetBrains.Annotations;
using UnityEngine;

namespace Core.AuraEffects
{
    [UsedImplicitly, CreateAssetMenu(fileName = "Aura Effect Spell Trigger", menuName = "Game Data/Spells/Auras/Effects/Spell Trigger", order = 4)]
    public class AuraEffectInfoSpellTrigger : AuraEffectInfo
    {
        [SerializeField, UsedImplicitly, Range(0.0f, 1.0f)]
        private float chance;
        [SerializeField, UsedImplicitly, Range(0.0f, 1.0f)]
        private float chancePerCombo;
        [SerializeField, UsedImplicitly]
        private bool isCasterTriggerTarget;
        [SerializeField, UsedImplicitly]
        private bool canCasterBeTriggerTarget;
        [SerializeField, UsedImplicitly]
        private SpellInfo triggeredSpell;
        [SerializeField, UsedImplicitly]
        private SpellTriggerFlags triggerFlags;
        [SerializeField, UsedImplicitly]
        private List<Condition> triggerConditions;

        public bool IsCasterTriggerTarget => isCasterTriggerTarget;
        public bool CanCasterBeTriggerTarget => canCasterBeTriggerTarget;
        public float Chance => chance;
        public float ChancePerCombo => chancePerCombo;
        public SpellInfo TriggeredSpell => triggeredSpell;
        public SpellTriggerFlags TriggerFlags => triggerFlags;
        public IReadOnlyList<Condition> TriggerConditions => triggerConditions;

        public override float Value => chance;
        public override AuraEffectType AuraEffectType => AuraEffectType.TriggerSpellChance;

        internal override AuraEffect CreateEffect(Aura aura, Unit caster, int index)
        {
            return new AuraEffectSpellTrigger(aura, this, index, Value);
        }
    }
}