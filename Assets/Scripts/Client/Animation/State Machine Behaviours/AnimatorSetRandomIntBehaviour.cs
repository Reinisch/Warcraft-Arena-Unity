using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Animations;

namespace Client
{
    public class AnimatorSetRandomIntBehaviour : StateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private string parameterName;
        [SerializeField, UsedImplicitly] private int minInclusive;
        [SerializeField, UsedImplicitly] private int maxExclusive;

        private int parameterHash;

        [UsedImplicitly]
        private void OnEnable()
        {
            parameterHash = Animator.StringToHash(parameterName);
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
        {
            animator.SetInteger(parameterHash, Random.Range(minInclusive, maxExclusive));
        }
    }
}