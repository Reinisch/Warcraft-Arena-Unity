using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    internal class EffectEntity : MonoBehaviour, IEffectEntity
    {
        [SerializeField, UsedImplicitly] private List<EffectBehaviour> behaviours;

        private EffectSettings effectSettings;
        private Quaternion originalRotation;

        internal EffectState State { get; private set; }
        internal long PlayId { get; private set; }

        public Transform Transform => transform;
        public bool KeepOriginalRotation { private get; set; }
        public bool KeepAliveWithNoParticles { get; set; }

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
            if (!State.IsPlaying())
                return;

            bool keepAlive = false;
            foreach (EffectBehaviour effectBehaviour in behaviours)
                effectBehaviour.DoUpdate(this, ref keepAlive);

            if (KeepOriginalRotation)
                transform.rotation = originalRotation;

            if (!keepAlive)
                Stop(PlayId, false);
        }

        internal void Play(long playId)
        {
            PlayId = playId;
            State = EffectState.Active;

            originalRotation = transform.rotation;

            foreach (EffectBehaviour effectBehaviour in behaviours)
                effectBehaviour.Play(PlayId);
        }

        private void Stop(long playId, bool isDestroyed)
        {
            if (!isDestroyed && !State.IsPlaying())
                return;

            if (isDestroyed)
            {
                effectSettings?.HandleStop(this, true);
                State = EffectState.Unused;
            }
            else if (PlayId == playId)
            {
                effectSettings?.HandleStop(this, false);
                State = EffectState.Idle;
            }
        }

        public bool IsPlaying(long playId) => State == EffectState.Active && playId == PlayId;

        public bool IsFading(long playId) => State == EffectState.Fading && playId == PlayId;

        public void Stop(long playId) => Stop(playId, false);

        public void Fade(long playId)
        {
            if (State == EffectState.Active && PlayId == playId)
            {
                effectSettings?.HandleFade(this);
                behaviours.ForEach(behaviour => behaviour.Fade());
                State = EffectState.Fading;
            }
        }

        public void Replay(long playId)
        {
            if (PlayId == playId && State.IsPlaying())
                behaviours.ForEach(behaviour => behaviour.Replay());
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            Stop(PlayId, true);
        }
    }
}
