using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Animations;

namespace Core
{
    [UsedImplicitly]
    public class UnitStateMachineBehaviour : StateMachineBehaviour, IUnitStateMachineBehaviour
    {
        protected UnitStateMachine StateMachine { get; private set; }
        protected Unit Unit => StateMachine.Unit;

        private bool active;

        void IUnitStateMachineBehaviour.Register(UnitStateMachine stateMachine)
        {
            StateMachine = stateMachine;
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

        public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            OnStart();
        }

        public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            OnExit();
        }
    }
}
