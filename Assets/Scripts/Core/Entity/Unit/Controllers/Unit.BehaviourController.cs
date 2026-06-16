using System;
using System.Collections.Generic;

namespace Core
{
    public abstract partial class Unit
    {
        protected class BehaviourController
        {
            private readonly List<IUnitBehaviour> activeBehaviours = new();
            private readonly Dictionary<Type, IUnitBehaviour> activeBehavioursByType = new();

            private World world;

            internal void DoUpdate(int deltaTime)
            {
                foreach (IUnitBehaviour unitBehaviour in activeBehaviours)
                    unitBehaviour.DoUpdate(deltaTime);
            }

            internal void HandleUnitAttach(Unit unit)
            {
                world = unit.World;

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
            }

            internal TUnitBehaviour FindBehaviour<TUnitBehaviour>()
            {
                return activeBehavioursByType.TryGetValue(typeof(TUnitBehaviour), out IUnitBehaviour behaviour) ? (TUnitBehaviour)behaviour : default;
            }

            internal void TryAddBehaviour(IUnitBehaviour unitBehaviour)
            {
                // Skip behaviours whose declared logic doesn't run under this instance's role.
                // Behaviours without ILogicBehaviour (e.g. local movement) always run.
                if (world != null && unitBehaviour is ILogicBehaviour logic && !ShouldRun(logic))
                    return;

                activeBehaviours.Add(unitBehaviour);
                activeBehavioursByType.Add(unitBehaviour.GetType(), unitBehaviour);
            }

            private bool ShouldRun(ILogicBehaviour behaviour)
            {
                return behaviour.HasServerLogic && world.HasServerLogic
                    || behaviour.HasClientLogic && world.HasClientLogic;
            }
        }
    }
}