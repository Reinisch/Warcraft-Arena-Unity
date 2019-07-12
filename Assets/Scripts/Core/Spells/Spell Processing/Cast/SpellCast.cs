namespace Core
{
    public class SpellCast
    {
        internal enum HandleMode
        {
            Started,
            Finished
        }

        private readonly IUnitState unitState;
        private readonly Unit caster;
        private Spell currentSpell;

        internal bool IsCasting
        {
            get
            {
                if (caster.IsOwner)
                    return currentSpell != null && currentSpell.ExecutionState == SpellExecutionState.Casting;

                return unitState.SpellCast.Id != 0;
            }
        }

        internal SpellCast(Unit caster)
        {
            this.caster = caster;
            unitState = caster.EntityState;
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
                    unitState.SpellCast.Id = spell.SpellInfo.Id;
                    unitState.SpellCast.ServerFrame = BoltNetwork.ServerFrame;
                    unitState.SpellCast.CastTime = spell.CastTime;
                    break;
                case HandleMode.Finished:
                    unitState.SpellCast.Id = 0;
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