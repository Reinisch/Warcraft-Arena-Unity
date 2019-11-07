using System.Collections.Generic;
using UnityEngine;

namespace Core.Conditions
{
    /// <summary>
    /// Base for any game condition.
    /// </summary>
    public abstract class Condition : ScriptableObject
    {
        protected Unit SourceUnit { get; private set; }
        protected Unit TargetUnit { get; private set; }
        protected Spell Spell { get; private set; }
        protected SpellInfo SpellInfo { get; private set; }

        private readonly HashSet<Condition> usedConditions = new HashSet<Condition>();

        /// <summary>
        /// Returns true if condition has needed data to be even considered valid or not.
        /// </summary>
        protected virtual bool IsApplicable => true;

        /// <summary>
        /// Returns true if condition is satisfied.
        /// </summary>
        protected virtual bool IsValid => true;

        public bool IsApplicableAndInvalid(Unit source = null, Unit target = null, Spell spell = null, SpellInfo spellInfo = null)
        {
            SetInternalResources(this, source, target, spell, spellInfo);

            bool result = IsApplicable && !IsValid;

            foreach (Condition condition in usedConditions)
                FreeInternalResources(condition);

            usedConditions.Clear();

            return result;
        }

        public bool IsApplicableAndValid(Unit source = null, Unit target = null, Spell spell = null, SpellInfo spellInfo = null)
        {
            SetInternalResources(this, source, target, spell, spellInfo);

            bool result = IsApplicable && IsValid;

            foreach (Condition condition in usedConditions)
                FreeInternalResources(condition);

            usedConditions.Clear();

            return result;
        }

        protected bool IsOtherApplicable(Condition condition)
        {
            SetResources(condition);

            return condition.IsApplicable;
        }

        protected bool IsOtherValid(Condition condition)
        {
            SetResources(condition);

            return condition.IsValid;
        }

        private void SetResources(Condition condition)
        {
            if(!usedConditions.Contains(condition))
                SetInternalResources(condition, SourceUnit, TargetUnit, Spell, SpellInfo);
        }

        private void SetInternalResources(Condition condition, Unit source = null, Unit target = null, Spell spell = null, SpellInfo spellInfo = null)
        {
            condition.SourceUnit = source;
            condition.TargetUnit = target;
            condition.Spell = spell;
            condition.SpellInfo = spellInfo ?? spell?.SpellInfo;

            usedConditions.Add(condition);
        }

        private void FreeInternalResources(Condition condition)
        {
            condition.SourceUnit = null;
            condition.TargetUnit = null;
            condition.Spell = null;
            condition.SpellInfo = null;
        }
    }
}
