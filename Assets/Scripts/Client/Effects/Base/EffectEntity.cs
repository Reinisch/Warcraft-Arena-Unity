using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    internal class EffectEntity : MonoBehaviour, IEffectEntity
    {
        [SerializeField, UsedImplicitly] private ParticleSystem mainParticleSystem;

        private EffectSettings effectSettings;

        internal EffectState State { get; private set; }
        internal long PlayId { get; private set; }

        public Transform Transform => transform;

        internal void Initialize(EffectSettings effectSettings)
        {
            State = EffectState.Idle;
            this.effectSettings = effectSettings;
        }

        internal void Deinitialize()
        {
            State = EffectState.Unused;
            effectSettings = null;
        }

        internal void DoUpdate()
        {
            if (State == EffectState.Active && !mainParticleSystem.IsAlive(true))
                Stop(PlayId, false);
        }

        internal void Play(long playId)
        {
            PlayId = playId;

            mainParticleSystem.Stop(true);
            mainParticleSystem.Play(true);

            State = EffectState.Active;
        }

        private void Stop(long playId, bool isDestroyed)
        {
            if (PlayId == playId || isDestroyed)
            {
                effectSettings?.StopEffect(this, isDestroyed);

                State = isDestroyed ? EffectState.Unused : EffectState.Idle;
            }
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            Stop(PlayId, true);
        }
    }
}
