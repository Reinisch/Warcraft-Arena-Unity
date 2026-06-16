using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

namespace Client
{
    public class EffectBehaviourReplayable : EffectBehaviour
    {
        [SerializeField, UsedImplicitly] private List<ParticleSystem> replayableSystems;

        protected override void OnPlay()
        {
            base.OnPlay();

            replayableSystems.ForEach(system => system.Play());
        }

        protected override void OnReplay()
        {
            OnPlay();
        }

#if UNITY_EDITOR
        [ContextMenu("Collect"), UsedImplicitly]
        private void Collect()
        {
            replayableSystems = new List<ParticleSystem>(GetComponentsInChildren<ParticleSystem>());
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}
