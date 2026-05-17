using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class AnimatorSetIntBehaviour : StateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string parameterName;
        [SerializeField, UsedImplicitly] private int valueOnEnter;
        [SerializeField, UsedImplicitly] private int valueOnExit;
        [SerializeField, UsedImplicitly] private bool setOnEnter = true;
        [SerializeField, UsedImplicitly] private bool setOnExit = true;

        private int parameterHash;

        [UsedImplicitly]
        private void OnEnable()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setOnEnter)
                animator.SetInteger(parameterHash, valueOnEnter);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (setOnExit)
                if (animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash != stateInfo.shortNameHash)
                    animator.SetInteger(parameterHash, valueOnExit);
        }
    }
}
