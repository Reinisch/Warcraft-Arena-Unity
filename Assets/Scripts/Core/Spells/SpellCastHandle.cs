namespace Core
{
    public readonly struct SpellCastHandle
    {
        private readonly Spell.SpellRuntime onCastState;
        private readonly SpellCastResult result;

        public SpellCastResult Result => result;
        public SpellExecutionState ExecutionState => onCastState.ExecutionState;
        public int TotalDamage => onCastState.TotalDamage;
        public int EffectDamage => onCastState.EffectDamage;
        public int EffectHealing => onCastState.EffectHealing;
        public bool IsSuccess => result == SpellCastResult.Success;

        internal SpellCastHandle(SpellCastResult result, in Spell.SpellRuntime onCastState)
        {
            this.result = result;
            this.onCastState = onCastState;
        }

        public static implicit operator SpellCastResult(SpellCastHandle handle) => handle.result;
    }
}
