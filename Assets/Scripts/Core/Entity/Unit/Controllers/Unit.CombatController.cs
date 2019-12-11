using Common;

namespace Core
{
    public abstract partial class Unit
    {
        internal class CombatController : IUnitBehaviour
        {
            public bool HasClientLogic => false;
            public bool HasServerLogic => true;

            private Unit unit;
            private TimeTracker combatTimer = new TimeTracker();

            public bool InCombat => unit.HasFlag(UnitFlags.InCombat);

            void IUnitBehaviour.DoUpdate(int deltaTime)
            {
                if (InCombat)
                {
                    combatTimer.Update(deltaTime);

                    if (combatTimer.Passed)
                        ResetCombat();
                }
            }

            void IUnitBehaviour.HandleUnitAttach(Unit unit)
            {
                this.unit = unit;
            }

            void IUnitBehaviour.HandleUnitDetach()
            {
                ResetCombat();

                unit = null;
            }

            internal void StartCombatWith(Unit enemy)
            {
                RefreshCombatState();
                enemy.Combat.RefreshCombatState();
            }

            internal void RefreshCombatState()
            {
                if (!unit.IsAlive)
                    return;

                combatTimer.Reset(5000);
                unit.SetFlag(UnitFlags.InCombat);
            }

            internal void ResetCombat()
            {
                combatTimer.Reset(0);
                unit.RemoveFlag(UnitFlags.InCombat);
            }
        }
    }
}