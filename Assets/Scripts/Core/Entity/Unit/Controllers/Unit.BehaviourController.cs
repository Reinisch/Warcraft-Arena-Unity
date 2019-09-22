using System;
using System.Collections.Generic;

namespace Core
{
    public abstract partial class Unit
    {
        protected class BehaviourController
        {
            private readonly List<IUnitBehaviour> activeBehaviours = new List<IUnitBehaviour>();
            private readonly Dictionary<Type, IUnitBehaviour> activeBehavioursByType = new Dictionary<Type, IUnitBehaviour>();
            private Unit unit;

            internal void DoUpdate(int deltaTime)
            {
                foreach (IUnitBehaviour unitBehaviour in activeBehaviours)
                    unitBehaviour.DoUpdate(deltaTime);
            }

            internal void HandleUnitAttach(Unit unit)
            {
                this.unit = unit;

                unit.AddBehaviours(this);

                foreach (UnitBehaviour unitBehaviour in unit.unitBehaviours)
                    TryAddBehaviour(unitBehaviour);

                for (int i = 0; i < activeBehaviours.Count; i++)
                    activeBehaviours[i].HandleUnitAttach(unit);
            }

            internal void HandleUnitDetach()
            {
                for (int i = activeBehaviours.Count - 1; i >= 0; i--)
                    activeBehaviours[i].HandleUnitDetach();

                activeBehaviours.Clear();
                activeBehavioursByType.Clear();

                unit = null;
            }

            internal TUnitBehaviour FindBehaviour<TUnitBehaviour>()
            {
                return activeBehavioursByType.TryGetValue(typeof(TUnitBehaviour), out IUnitBehaviour behaviour) ? (TUnitBehaviour)behaviour : default;
            }

            internal void TryAddBehaviour(IUnitBehaviour unitBehaviour)
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