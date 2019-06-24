using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Animations;

namespace Client
{
    public class AnimatorSetBoolBehaviour : StateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string parameterName;
        [SerializeField, UsedImplicitly] private bool valueOnEnter;
        [SerializeField, UsedImplicitly] private bool valueOnExit;
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
                animator.SetBool(parameterHash, valueOnEnter);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            if (setOnExit)
                animator.SetBool(parameterHash, valueOnExit);
        }
    }
}
