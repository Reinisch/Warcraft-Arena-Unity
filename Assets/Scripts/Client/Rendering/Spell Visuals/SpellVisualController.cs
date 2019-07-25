using System;
using System.Collections.Generic;
using Client.Spells;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public partial class RenderingReference
    {
        [Serializable]
        private partial class SpellVisualController
        {
            [SerializeField, UsedImplicitly] private EffectTagType defaultTargetTag;

            private static EffectTagType DefaultTargetTag { get; set; }
            private readonly List<SpellVisualProjectile> activeProjectiles = new List<SpellVisualProjectile>();

            public void Initialize()
            {
                DefaultTargetTag = defaultTargetTag;
            }

            public void Deinitialize()
            {
                activeProjectiles.ForEach(visual => visual.HandleFinish(true));
                activeProjectiles.Clear();
            }

            public void DoUpdate(float deltaTime)
            {
                for (int i = activeProjectiles.Count - 1; i >= 0; i--)
                    if (activeProjectiles[i].DoUpdate())
                    {
                        activeProjectiles[i].HandleFinish(false);
                        activeProjectiles.RemoveAt(i);
                    }
            }

            public void SpawnVisual(UnitRenderer casterRenderer, UnitRenderer targetRenderer, EffectSpellSettings settings, int serverLaunchFrame, int delay)
            {
                var visualEntry = new SpellVisualProjectile(targetRenderer, settings, serverLaunchFrame, delay);
                if (visualEntry.HandleLaunch(casterRenderer))
                    activeProjectiles.Add(visualEntry);
            }

            public void HandleRendererDetach(UnitRenderer unitRenderer)
            {
                foreach (var projectile in activeProjectiles)
                    projectile.HandleRendererDetach(unitRenderer);
            }
        }
    }
}
