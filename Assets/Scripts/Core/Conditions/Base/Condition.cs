using UnityEngine;

namespace Core.Conditions
{
    /// <summary>
    /// Base for any game condition, implement <seealso cref="IsValid"/> with caution.
    /// </summary>
    public abstract class Condition : ScriptableObject
    {
        protected Player SourcePlayer { get; private set; }
        protected Player SourceUnit { get; private set; }

        /// <summary>
        /// Returns true if condition has needed data to be even considered valid or not.
        /// </summary>
        public virtual bool IsApplicable => true;

        /// <summary>
        /// Returns true if condition is satisfied, all implementations required to call base in the end to free resources.
        /// </summary>
        public virtual bool IsValid => FreeResources();

        public Condition WithSource(Player player) { SourceUnit = SourcePlayer = player; return this; }

        public Condition From(Condition condition)
        {
            SourceUnit = condition.SourceUnit;
            SourcePlayer = condition.SourcePlayer;
            return this;
        }

        private bool FreeResources()
        {
            SourceUnit = SourcePlayer = null;

            return true;
        }
    }
}
