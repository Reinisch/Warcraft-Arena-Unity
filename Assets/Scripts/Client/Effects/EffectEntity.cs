using Client.Effects;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class EffectEntity : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private ParticleSystem mainParticleSystem;

        public long PlayId { get; private set; }
        public EffectState State { get; internal set; }
        public EffectSettings EffectSettings { get; private set; }

        internal void Initialize(EffectSettings effectSettings)
        {
            State = EffectState.Idle;
            EffectSettings = effectSettings;
        }

        internal void Deinitialize()
        {
            State = EffectState.Unused;
            EffectSettings = null;
        }

        internal void DoUpdate()
        {
            if (State == EffectState.Active && !mainParticleSystem.IsAlive(true))
                Stop();
        }

        internal void Play(long playId)
        {
            mainParticleSystem.Stop(true);
            mainParticleSystem.Play(true);

            PlayId = playId;
            State = EffectState.Active;
        }

        private void Stop()
        {
            EffectSettings?.StopEffect(this, false);
        }

        [UsedImplicitly]
        private void OnDestroy()
        {
            EffectSettings?.StopEffect(this, true);
        }
    }
}
