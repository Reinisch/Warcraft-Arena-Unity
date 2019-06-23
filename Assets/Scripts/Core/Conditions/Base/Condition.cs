using UnityEngine;

namespace Core.Conditions
{
    /// <summary>
    /// Base for any game condition, implement <seealso cref="IsValid"/> with caution.
    /// </summary>
    public abstract class Condition : ScriptableObject
    {
        protected Unit SourceUnit { get; private set; }
        protected Unit TargetUnit { get; private set; }
        protected Spell Spell { get; private set; }

        /// <summary>
        /// Returns true if condition has needed data to be even considered valid or not.
        /// </summary>
        protected internal virtual bool IsApplicable => true;

        /// <summary>
        /// Returns true if condition is satisfied, all implementations required to call base in the end to free resources.
        /// </summary>
        protected internal virtual bool IsValid => FreeResources(this);

        public bool IsApplicableAndInvalid
        {
            get
            {
                bool isApplicable = IsApplicable;
                bool isValid = IsValid;

                return !isValid && isApplicable;
            }
        }

        public bool IsApplicableAndValid
        {
            get
            {
                bool isApplicable = IsApplicable;
                bool isValid = IsValid;

                return isValid && isApplicable;
            }
        }

        public Condition With(Unit source = null, Unit target = null, Spell spell = null)
        {
            return SetResources(source, target, spell);
        }

        public Condition From(Condition condition)
        {
            return With(condition.SourceUnit, condition.TargetUnit, condition.Spell);
        }

        protected virtual Condition SetResources(Unit source = null, Unit target = null, Spell spell = null)
        {
            SourceUnit = source;
            TargetUnit = target;
            Spell = spell;

            return this;
        }

        protected virtual bool FreeResources(Condition condition)
        {
            condition.SourceUnit = condition.TargetUnit = null;
            condition.Spell = null;

            return true;
        }
    }
}
