using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class UnitStateMachineBehaviour : StateMachineBehaviour, IUnitStateMachineBehaviour
    {
        protected UnitStateMachine StateMachine { get; private set; }
        protected Unit Unit => StateMachine.Unit;
        protected Animator StateAnimator => StateMachine.StateMachineAnimator;

        private bool active;

        void IUnitStateMachineBehaviour.Register(UnitStateMachine stateMachine)
        {
            StateMachine = stateMachine;

            OnRegister();
        }

        void IUnitStateMachineBehaviour.Unregister()
        {
            active = false;

            StateMachine = null;
        }

        void IUnitStateMachineBehaviour.DoUpdate(int deltaTime)
        {
            if (active)
                OnActiveUpdate(deltaTime);
            else
                OnDisabledUpdate(deltaTime);
        }

        protected virtual void OnRegister()
        {
        }

        protected virtual void OnStart()
        {
            active = true;
        }

        protected virtual void OnExit()
        {
            active = false;
        }

        protected virtual void OnActiveUpdate(int deltaTime)
        {
        }

        protected virtual void OnDisabledUpdate(int deltaTime)
        {
        }

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnStart();
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            OnExit();
        }
    }
}
