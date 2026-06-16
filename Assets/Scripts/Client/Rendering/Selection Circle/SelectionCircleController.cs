using Common;
using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Client
{
    public partial class RenderingReference
    {
        [Serializable]
        private partial class SelectionCircleController
        {
            [SerializeField, UsedImplicitly] private DecalProjector selectionCirclePrototype;
            [SerializeField, UsedImplicitly] private SelectionCircleSettings playerCircleSettings;
            [SerializeField, UsedImplicitly] private SelectionCircleSettings targetCircleSettings;

            private SelectionCircle playerCircle;
            private SelectionCircle targetCircle;
            private RenderingReference rendering;

            private readonly Action onPlayerTargetChanged;

            private SelectionCircleController()
            {
                onPlayerTargetChanged = OnPlayerTargetChanged;
            }

            public void Initialize(RenderingReference rendering)
            {
                this.rendering = rendering;
                GameObjectPool.PreInstantiate(selectionCirclePrototype, 2);

                playerCircle = new SelectionCircle(this, playerCircleSettings);
                targetCircle = new SelectionCircle(this, targetCircleSettings);
            }

            public void Deinitialize()
            {
                playerCircle.Dispose();
                targetCircle.Dispose();

                playerCircle = targetCircle = null;
            }

            public void HandleRendererAttach(UnitRenderer attachedRenderer)
            {
                playerCircle.HandleRendererAttach(attachedRenderer);
                targetCircle.HandleRendererAttach(attachedRenderer);
            }

            public void HandleRendererDetach(UnitRenderer detachedRenderer)
            {
                playerCircle.HandleRendererDetach(detachedRenderer);
                targetCircle.HandleRendererDetach(detachedRenderer);
            }

            public void HandlePlayerControlGained()
            {
                playerCircle.UpdateUnit(rendering.Player);
                targetCircle.UpdateUnit(rendering.Player.Target);

                rendering.eventBus.RegisterEvent(rendering.Player, GameEvents.UnitTargetChanged, onPlayerTargetChanged);
            }

            public void HandlePlayerControlLost()
            {
                playerCircle.UpdateUnit(null);
                targetCircle.UpdateUnit(null);

                rendering.eventBus.UnregisterEvent(rendering.Player, GameEvents.UnitTargetChanged, onPlayerTargetChanged);
            }

            private void OnPlayerTargetChanged()
            {
                targetCircle.UpdateUnit(rendering.Player.Target);
            }
        }
    }
}
