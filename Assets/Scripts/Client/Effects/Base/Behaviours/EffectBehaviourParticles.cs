using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class EffectBehaviourParticles : EffectBehaviour
    {
        [SerializeField, UsedImplicitly] 
        private List<ParticleSystem> trackedParticleSystems;

        protected override void OnPlay()
        {
            base.OnPlay();

            trackedParticleSystems.ForEach(system => system.Stop());
            trackedParticleSystems.ForEach(system => system.Play());
        }

        protected override void OnFade()
        {
            base.OnFade();

            trackedParticleSystems.ForEach(system => system.Stop());
        }

        protected override void OnUpdate(IEffectEntity effectEntity, float deltaTime, ref bool keepAlive)
        {
            base.OnUpdate(effectEntity, deltaTime, ref keepAlive);

            if (effectEntity.IsPlaying(PlayId) && effectEntity.KeepAliveWithNoParticles)
                keepAlive = true;
            else foreach (ParticleSystem system in trackedParticleSystems)
                keepAlive |= system.IsAlive(false);
        }

#if UNITY_EDITOR
        [ContextMenu("Collect"), UsedImplicitly]
        private void Collect()
        {
            trackedParticleSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
