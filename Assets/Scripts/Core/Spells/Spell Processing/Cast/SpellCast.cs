namespace Core
{
    public class SpellCast
    {
        internal enum HandleMode
        {
            Started,
            Finished
        }

        private readonly Unit caster;
        private readonly IUnitState casterState;
        private Spell currentSpell;

        internal bool IsCasting
        {
            get
            {
                if (caster.IsOwner)
                    return currentSpell != null && currentSpell.ExecutionState == SpellExecutionState.Casting;

                return casterState.SpellCast.Id != 0;
            }
        }

        public SpellCastState State => casterState.SpellCast;

        internal SpellCast(Unit caster, IUnitState casterState)
        {
            this.caster = caster;
            this.casterState = casterState;
        }

        internal void Detached()
        {
            Cancel();
        }

        internal void HandleSpellCast(Spell spell, HandleMode handleMode)
        {
            switch (handleMode)
            {
                case HandleMode.Started:
                    currentSpell = spell;
                    casterState.SpellCast.Id = spell.SpellInfo.Id;
                    casterState.SpellCast.ServerFrame = BoltNetwork.ServerFrame;
                    casterState.SpellCast.CastTime = spell.CastTime;
                    break;
                case HandleMode.Finished:
                    casterState.SpellCast.Id = 0;
                    currentSpell = null;
                    break;
            }
        }

        internal void Cancel()
        {
            if (currentSpell != null)
            {
                currentSpell.Cancel();

                HandleSpellCast(currentSpell, HandleMode.Finished);
            }
        }
    }
}