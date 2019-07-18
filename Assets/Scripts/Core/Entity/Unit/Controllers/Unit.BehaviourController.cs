using System;
using System.Collections.Generic;

namespace Core
{
    public abstract partial class Unit
    {
        private class BehaviourController
        {
            private readonly List<IUnitBehaviour> activeBehaviours = new List<IUnitBehaviour>();
            private readonly Dictionary<Type, IUnitBehaviour> activeBehavioursByType = new Dictionary<Type, IUnitBehaviour>();

            internal void DoUpdate(int deltaTime)
            {
                foreach (IUnitBehaviour unitBehaviour in activeBehaviours)
                    unitBehaviour.DoUpdate(deltaTime);
            }

            internal void HandleUnitAttach(Unit unit)
            {
                TryAddBehaviour(unit.AttributeUnitController, unit);
                TryAddBehaviour(unit.ThreatUnitController, unit);
                TryAddBehaviour(unit.ApplicationAuraController, unit);
                TryAddBehaviour(unit.VisibleAuraController, unit);

                foreach (UnitBehaviour unitBehaviour in unit.unitBehaviours)
                    TryAddBehaviour(unitBehaviour, unit);

                for (int i = 0; i < activeBehaviours.Count; i++)
                    activeBehaviours[i].HandleUnitAttach(unit);
            }

            internal void HandleUnitDetach()
            {
                for (int i = activeBehaviours.Count - 1; i >= 0; i--)
                    activeBehaviours[i].HandleUnitDetach();

                activeBehaviours.Clear();
                activeBehavioursByType.Clear();
            }

            internal TUnitBehaviour FindBehaviour<TUnitBehaviour>()
            {
                return activeBehavioursByType.TryGetValue(typeof(TUnitBehaviour), out IUnitBehaviour behaviour) ? (TUnitBehaviour)behaviour : default;
            }

            private void TryAddBehaviour(IUnitBehaviour unitBehaviour, Unit unit)
            {
                if (unitBehaviour.HasServerLogic && unit.World.HasServerLogic || unitBehaviour.HasClientLogic && unit.World.HasClientLogic)
                {
                    activeBehaviours.Add(unitBehaviour);
                    activeBehavioursByType.Add(unitBehaviour.GetType(), unitBehaviour);
                }
            }
        }
    }
}