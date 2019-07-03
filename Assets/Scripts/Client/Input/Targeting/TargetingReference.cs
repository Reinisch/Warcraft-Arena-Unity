using System;
using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// Handles client-side targeting for local player.
    /// </summary>
    [CreateAssetMenu(fileName = "Targeting Reference", menuName = "Game Data/Scriptable/Targeting", order = 11)]
    public partial class TargetingReference : ScriptableReferenceClient
    {
        [SerializeField, UsedImplicitly] private InputReference input;
        [SerializeField, UsedImplicitly] private CameraReference cameraReference;
        [SerializeField, UsedImplicitly] private TargetingSettings targetingSettings;

        private readonly List<Unit> previousTargets = new List<Unit>();

        protected override void OnRegistered()
        {
            base.OnRegistered();

            previousTargets.Clear();
        }

        protected override void OnUnregister()
        {
            previousTargets.Clear();

            base.OnUnregister();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (!Player.ExistsIn(World))
                return;

            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = cameraReference.WarcraftCamera.Camera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out var hit, float.MaxValue, PhysicsReference.Mask.Characters | PhysicsReference.Mask.Ground))
                    if (World.UnitManager.TryFind(hit.collider, out Unit target))
                        input.SelectTarget(target);
            }
        }

        protected override void OnWorldInitialized(WorldManager world)
        {
            base.OnWorldInitialized(world);

            previousTargets.Clear();

            world.UnitManager.EventEntityDetach += OnEntityDetach;
        }

        protected override void OnWorldDeinitializing(WorldManager world)
        {
            world.UnitManager.EventEntityDetach -= OnEntityDetach;

            previousTargets.Clear();

            base.OnWorldDeinitializing(world);
        }

        private void OnEntityDetach(Unit unit)
        {
            previousTargets.Remove(unit);
        }

        public void SelectTarget(TargetingOptions options)
        {
            if (!Player.ExistsIn(World))
                return;

            switch (options.Mode)
            {
                case TargetingMode.Normal:
                    using (var selector = new PlayerTargetSelector(targetingSettings, Player, options, previousTargets))
                    {
                        Player.Map.VisitInRadius(Player, targetingSettings.TargetRange, selector);
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
                    if(Player.Target != Player)
                        input.SelectTarget(Player);
                    break;
                case TargetingMode.Clear:
                    if(Player.Target != null)
                        input.SelectTarget(null);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(options.Mode), $"Unknown targeting kind: {options.Mode}");
            }
        }
    }
}
