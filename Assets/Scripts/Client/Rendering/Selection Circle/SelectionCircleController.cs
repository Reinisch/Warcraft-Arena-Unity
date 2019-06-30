using System;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    [Serializable]
    public partial class SelectionCircleController
    {
        [SerializeField, UsedImplicitly] private Projector selectionCirclePrototype;
        [SerializeField, UsedImplicitly] private RenderingReference renderingReference;
        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;
        [SerializeField, UsedImplicitly] private SelectionCircleSettings playerCircleSettings;
        [SerializeField, UsedImplicitly] private SelectionCircleSettings targetCircleSettings;

        private SelectionCircle playerCircle;
        private SelectionCircle targetCircle;

        private Player player;

        private readonly Action onPlayerTargetChanged;

        private SelectionCircleController()
        {
            onPlayerTargetChanged = OnPlayerTargetChanged;
        }

        public void Initialize()
        {
            GameObjectPool.PreInstantiate(selectionCirclePrototype, 2);

            playerCircle = new SelectionCircle(this, playerCircleSettings);
            targetCircle = new SelectionCircle(this, targetCircleSettings);

            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);
        }

        public void Deinitialize()
        {
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);

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

        private void OnPlayerControlGained(Player player)
        {
            this.player = player;

            playerCircle.UpdateUnit(player);
            targetCircle.UpdateUnit(player.Target);

            EventHandler.RegisterEvent(player, GameEvents.UnitTargetChanged, onPlayerTargetChanged);
        }

        private void OnPlayerControlLost(Player player)
        {
            this.player = null;

            playerCircle.UpdateUnit(null);
            targetCircle.UpdateUnit(null);

            EventHandler.UnregisterEvent(player, GameEvents.UnitTargetChanged, onPlayerTargetChanged);
        }

        private void OnPlayerTargetChanged()
        {
            targetCircle.UpdateUnit(player.Target);
        }
    }
}
