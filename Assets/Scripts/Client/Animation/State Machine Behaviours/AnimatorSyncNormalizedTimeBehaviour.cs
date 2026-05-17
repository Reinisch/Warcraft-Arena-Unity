using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class AnimatorSyncNormalizedTimeBehaviour : StateMachineBehaviour
    {
        [SerializeField, UsedImplicitly] private int otherLayerIndex;

        private bool alreadySynced;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (alreadySynced)
                alreadySynced = false;
            else
            {
                AnimatorStateInfo otherStateInfo = animator.GetCurrentAnimatorStateInfo(otherLayerIndex);

                if (otherStateInfo.shortNameHash == stateInfo.shortNameHash)
                {
                    alreadySynced = true;
                    animator.CrossFade(stateInfo.shortNameHash, 0.2f, layerIndex, otherStateInfo.normalizedTime);
                }
            }
        }
    }
}
