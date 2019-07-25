using UnityEngine;

namespace Client
{
    public abstract class EffectBehaviour : MonoBehaviour
    {
        protected long PlayId { get; private set; }

        internal void Play(long playId)
        {
            PlayId = playId;

            OnPlay();
        } 

        internal void Replay() => OnReplay();

        internal void Fade() => OnFade();

        internal void DoUpdate(IEffectEntity effectEntity, ref bool keepAlive) => OnUpdate(effectEntity, ref keepAlive);

        protected virtual void OnPlay()
        {
        }

        protected virtual void OnReplay()
        {
        }

        protected virtual void OnFade()
        {
        }

        protected virtual void OnUpdate(IEffectEntity effectEntity, ref bool keepAlive)
        {
        }
    }
}
