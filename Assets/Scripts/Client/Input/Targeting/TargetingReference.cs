using System;
using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    /// <summary>
    /// Handles client-side targeting for local player.
    /// </summary>
    [CreateAssetMenu(fileName = "Targeting Reference", menuName = "Game Data/Scriptable/Targeting", order = 11)]
    public partial class TargetingReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private InputReference input;
        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;
        [SerializeField, UsedImplicitly] private TargetingSettings targetingSettings;

        private readonly List<Unit> previousTargets = new List<Unit>();
        private WorldManager world;
        private Player player;

        protected override void OnRegistered()
        {
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);

            previousTargets.Clear();

            world = null;
            player = null;
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            world = worldManager;

            worldManager.UnitManager.EventEntityDetach += OnEntityDetach;
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            worldManager.UnitManager.EventEntityDetach -= OnEntityDetach;

            world = null;
        }

        private void OnPlayerControlGained(Player player)
        {
            this.player = player;
        }

        private void OnPlayerControlLost(Player player)
        {
            this.player = null;

            previousTargets.Clear();
        }

        private void OnEntityDetach(Unit unit)
        {
            previousTargets.Remove(unit);
        }

        public void SelectTarget(TargetingOptions options)
        {
            if (!player.ExistsIn(world))
                return;

            switch (options.Mode)
            {
                case TargetingMode.Normal:
                    using (var selector = new PlayerTargetSelector(targetingSettings, player, options, previousTargets))
                    {
                        player.Map.VisitInRadius(player, targetingSettings.TargetRange, selector);
                        input.SelectTarget(selector.BestTarget);
                        if (selector.BestTarget != null)
                        {
                            previousTargets.Remove(selector.BestTarget);
                            previousTargets.Add(selector.BestTarget);
                        }
                        else
                            previousTargets.Clear();
                    }
                    break;
                case TargetingMode.Self:
                    input.SelectTarget(player);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Mode), $"Unknown targeting kind: {options.Mode}");
            }
        }
    }
}
