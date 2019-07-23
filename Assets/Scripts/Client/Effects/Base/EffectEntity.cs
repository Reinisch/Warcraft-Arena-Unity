using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    internal class EffectEntity : MonoBehaviour, IEffectEntity
    {
        [SerializeField, UsedImplicitly] private ParticleSystem mainParticleSystem;
        [SerializeField, UsedImplicitly] private List<ParticleSystem> replayableSystems;

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
            if (!isDestroyed && State != EffectState.Active)
                return;

            if (PlayId == playId || isDestroyed)
            {
                effectSettings?.StopEffect(this, isDestroyed);

                State = isDestroyed ? EffectState.Unused : EffectState.Idle;
            }
        }

        public bool IsPlaying(long playId) => State == EffectState.Active && playId == PlayId;

        public void Stop(long playId) => Stop(playId, false);

        public void Replay(long playId)
        {
            if (PlayId == playId || State == EffectState.Active)
                replayableSystems.ForEach(system => system.Play());
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            Stop(PlayId, true);
        }
    }
}
