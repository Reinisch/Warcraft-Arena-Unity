using System;
using System.Collections.Generic;
using Client.Spells;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public partial class SpellVisualController
    {
        [SerializeField, UsedImplicitly] private EffectTagType defaultTargetTag;

        private static EffectTagType DefaultTargetTag { get; set; }
        private readonly List<SpellVisualEntry> activeVisuals = new List<SpellVisualEntry>();

        public void Initialize()
        {
            DefaultTargetTag = defaultTargetTag;
        }

        public void Deinitialize()
        {
            activeVisuals.ForEach(visual => visual.HandleFinish());
            activeVisuals.Clear();
        }
        
        public void DoUpdate(float deltaTime)
        {
            for (int i = activeVisuals.Count - 1; i >= 0; i--)
                if (activeVisuals[i].DoUpdate(deltaTime))
                {
                    activeVisuals[i].HandleFinish();
                    activeVisuals.RemoveAt(i);
                }
        }

        public void SpawnVisual(UnitRenderer casterRenderer, UnitRenderer targetRenderer, EffectSpellSettings settings, int serverLaunchFrame, int delay)
        {
            var visualEntry = new SpellVisualEntry(casterRenderer, targetRenderer, settings, serverLaunchFrame, delay);
            if (visualEntry.HandleLaunch())
                activeVisuals.Add(visualEntry);
        }
    }
}
