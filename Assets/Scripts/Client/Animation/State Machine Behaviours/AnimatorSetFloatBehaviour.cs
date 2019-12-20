using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Animations;

namespace Client
{
    public class AnimatorSetFloatBehaviour : StateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string parameterName;
        [SerializeField, UsedImplicitly] private float valueOnEnter;
        [SerializeField, UsedImplicitly] private float valueOnExit;
        [SerializeField, UsedImplicitly] private bool setOnEnter = true;
        [SerializeField, UsedImplicitly] private bool setOnExit = true;

        private int parameterHash;

        [UsedImplicitly]
        private void OnEnable()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (setOnEnter)
                animator.SetFloat(parameterHash, valueOnEnter);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (setOnExit)
                if (animator.GetCurrentAnimatorStateInfo(layerIndex).shortNameHash != stateInfo.shortNameHash)
                    animator.SetFloat(parameterHash, valueOnExit);
        }
    }
}
