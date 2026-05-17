using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class AnimatorResetTriggerBehaviour : StateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string parameterName;
        [SerializeField, UsedImplicitly] private bool resetOnEnter;
        [SerializeField, UsedImplicitly] private bool resetOnExit;

        private int parameterHash;

        [UsedImplicitly]
        private void OnEnable()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(resetOnEnter)
                animator.ResetTrigger(parameterHash);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (resetOnExit)
                animator.ResetTrigger(parameterHash);
        }
    }
}
