using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    public sealed class UnitStateMachine : IUnitAIModel
    {
        private readonly List<IUnitStateMachineBehaviour> behaviours = new List<IUnitStateMachineBehaviour>();

        private UnitInfoAIStateMachine StateMachineInfo { get; }
        private Animator StateMachineAnimator { get; set; }

        public Unit Unit { get; private set; }

        public UnitStateMachine(UnitInfoAIStateMachine stateMachineInfo)
        {
            StateMachineInfo = stateMachineInfo;
        }

        void IUnitAIModel.Register(Unit unit)
        {
            StateMachineAnimator = GameObjectPool.Take(StateMachineInfo.Prototype);
            StateMachineAnimator.transform.SetParent(unit.transform, false);
            StateMachineAnimator.transform.localPosition = Vector3.zero;

            behaviours.AddRange(StateMachineAnimator.GetBehaviours<UnitStateMachineBehaviour>());
            foreach (IUnitStateMachineBehaviour behaviour in behaviours)
                behaviour.Register(this);

            Unit = unit;
        }

        void IUnitAIModel.Unregister()
        {
            foreach (IUnitStateMachineBehaviour behaviour in behaviours)
                behaviour.Unregister();

            behaviours.Clear();

            GameObjectPool.Return(StateMachineAnimator, false);
            StateMachineAnimator = null;
            Unit = null;
        }

        void IUnitAIModel.DoUpdate(int deltaTime)
        {
            foreach (IUnitStateMachineBehaviour behaviour in behaviours)
                behaviour.DoUpdate(deltaTime);
        }
    }
}
