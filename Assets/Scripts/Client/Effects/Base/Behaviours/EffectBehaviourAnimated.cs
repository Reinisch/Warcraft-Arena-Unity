using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class EffectBehaviourAnimated : EffectBehaviour
    {
        [SerializeField, UsedImplicitly] private Animator animator;
        [SerializeField, UsedImplicitly] private string playState;
        [SerializeField, UsedImplicitly] private string idleState;
        [SerializeField, UsedImplicitly] private string fadeState;
        [SerializeField, UsedImplicitly] private bool playWhileEnityActive = true;
        [SerializeField, UsedImplicitly] private bool hasPlayState;
        [SerializeField, UsedImplicitly] private bool hasFadeState;
        [SerializeField, UsedImplicitly] private bool hasIdleState;
        [SerializeField, UsedImplicitly] private bool replayPlayState;

        private int playHash = -1;
        private int idleHash = -1;
        private int fadeHash = -1;

        protected override void OnPlay()
        {
            base.OnPlay();

            if (hasPlayState && playHash == -1)
                playHash = Animator.StringToHash(playState);

            if (hasFadeState && fadeHash == -1)
                fadeHash = Animator.StringToHash(fadeState);

            if (hasIdleState && idleHash == -1)
                idleHash = Animator.StringToHash(idleState);

            if (hasPlayState)
                animator.Play(playHash, 0);
        }

        protected override void OnReplay()
        {
            base.OnReplay();

            if (hasPlayState && replayPlayState)
                animator.Play(playHash, 0);
        }

        protected override void OnFade()
        {
            base.OnFade();

            if (hasFadeState)
            {
                animator.Play(fadeHash, 0);
                animator.Update(0.0f);
            }
        }

        protected override void OnUpdate(IEffectEntity effectEntity, ref bool keepAlive)
        {
            base.OnUpdate(effectEntity, ref keepAlive);

            if (playWhileEnityActive)
            {
                if (effectEntity.IsPlaying(PlayId))
                    keepAlive = true;
                else if (hasFadeState && effectEntity.IsFading(PlayId) && fadeHash == animator.GetCurrentAnimatorStateInfo(0).shortNameHash)
                    keepAlive = true;
            }
            else
            {
                int currentHash = animator.GetCurrentAnimatorStateInfo(0).shortNameHash;
                if (currentHash == playHash || currentHash == idleHash || currentHash == fadeHash)
                    keepAlive = true;
            }
        }
    }
}
