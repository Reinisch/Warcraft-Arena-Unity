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
        private Spell currentSpell;

        internal bool IsCasting => currentSpell != null && currentSpell.ExecutionState == SpellExecutionState.Casting;

        internal SpellCast(Unit owner)
        {
            unitState = owner.EntityState;
        }

        internal void Dispose()
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