using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class AnimatorSetBoolBehaviour : StateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string parameterName;
        [SerializeField, UsedImplicitly] private bool valueOnEnter;
        [SerializeField, UsedImplicitly] private bool valueOnExit;
        [SerializeField, UsedImplicitly] private bool valueOnMachineEnter;
        [SerializeField, UsedImplicitly] private bool valueOnMachineExit;
        [SerializeField, UsedImplicitly] private bool setOnEnter = true;
        [SerializeField, UsedImplicitly] private bool setOnExit = true;
        [SerializeField, UsedImplicitly] private bool setOnMachineEnter;
        [SerializeField, UsedImplicitly] private bool setOnMachineExit;

        private int parameterHash;

        [UsedImplicitly]
        private void OnEnable()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setOnEnter)
                animator.SetBool(parameterHash, valueOnEnter);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setOnExit)
                if (animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash != stateInfo.shortNameHash)
                    animator.SetBool(parameterHash, valueOnExit);
        }

        public override void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (setOnMachineEnter)
                animator.SetBool(parameterHash, valueOnMachineEnter);
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            if (setOnMachineExit)
                animator.SetBool(parameterHash, valueOnMachineExit);
        }
    }
}
