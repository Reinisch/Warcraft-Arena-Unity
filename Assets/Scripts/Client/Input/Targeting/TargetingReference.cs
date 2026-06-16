using System;
using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public partial class TargetingReference : ScriptableReferenceClient
    {
        [Inject] private InputReference input;
        [Inject] private RenderingReference rendering;
        [Inject] private CameraReference cameraReference;

        [SerializeField, UsedImplicitly] 
        private TargetingSettings targetingSettings;

        private readonly List<Unit> previousTargets = new();

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

            if (!Player.ExistsIn(World) || Player.MovementMode != MovementMode.Rpg)
                return;

            if (input.LeftClickDown && !input.RightClickPressed && !InterfaceUtils.IsPointerOverUI)
            {
                Ray ray = cameraReference.WarcraftCamera.Camera.ScreenPointToRay(input.MousePosition);
                if (Physics.Raycast(ray, out var hit, float.MaxValue, PhysicsReference.Mask.Interactable | PhysicsReference.Mask.Ground))
                    if (rendering.TryFindRendererByHitBox(hit.collider, out UnitRenderer unitRenderer))
                        input.SelectTarget(unitRenderer.Unit);
            }
        }

        public override void OnWorldStateChanged(bool created)
        {
            if (created)
            {
                base.OnWorldStateChanged(true);

                previousTargets.Clear();

                World.UnitManager.EventEntityDetach += OnEntityDetach;
            }
            else
            {
                World.UnitManager.EventEntityDetach -= OnEntityDetach;

                previousTargets.Clear();

                base.OnWorldStateChanged(false);
            }
        }

        private void OnEntityDetach(Unit unit)
        {
            previousTargets.Remove(unit);
        }

        public void SelectTarget(TargetingOptions options)
        {
            if (!Player.ExistsIn(World) || Player.MovementMode != MovementMode.Rpg)
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
