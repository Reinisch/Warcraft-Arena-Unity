using System;
using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public partial class RenderingReference
    {
        [Serializable]
        public class UnitRendererController
        {
            [SerializeField, UsedImplicitly] private UnitRenderer unitRendererPrototype;

            private readonly Dictionary<ulong, UnitRenderer> unitRenderersById = new();
            private readonly List<UnitRenderer> unitRenderers = new();
            private readonly List<UnitModel> fadingModels = new();
            private readonly List<IUnitRendererHandler> unitRendererHandlers = new();

            private RenderingReference rendering;

            public void Initialize(RenderingReference rendering)
            {
                this.rendering = rendering;
                Assert.IsTrue(unitRendererHandlers.Count == 0);
                Assert.IsTrue(unitRenderers.Count == 0);
                Assert.IsTrue(unitRenderersById.Count == 0);

                rendering.World.UnitManager.EventEntityAttached += OnEventEntityAttached;
                rendering.World.UnitManager.EventEntityDetach += OnEventEntityDetach;
                rendering.eventBus.RegisterEvent<WorldEntity, bool>(rendering.World, GameEvents.ServerVisibilityChanged, OnServerVisibilityChanged);
            }

            public void Deinitialize()
            {
                // Deinitialize can be called (on quit) without a prior Initialize if startup aborted early.
                if (rendering == null)
                    return;

                rendering.eventBus.UnregisterEvent<WorldEntity, bool>(rendering.World, GameEvents.ServerVisibilityChanged, OnServerVisibilityChanged);
                rendering.World.UnitManager.EventEntityAttached -= OnEventEntityAttached;
                rendering.World.UnitManager.EventEntityDetach -= OnEventEntityDetach;

                foreach (UnitRenderer unitRenderer in unitRenderers)
                    unitRenderer.Detach(UnitModelReplacementMode.Complete);

                foreach (UnitModel fadingModel in fadingModels)
                    fadingModel.Deinitialize();

                unitRenderersById.Clear();
                unitRenderers.Clear();
                fadingModels.Clear();
            }

            public void DoUpdate(float deltaTime)
            {
                foreach (var unitRenderer in unitRenderers)
                    unitRenderer.DoUpdate(deltaTime);

                for (int i = fadingModels.Count - 1; i >= 0; i--)
                {
                    fadingModels[i].DoUpdate(null, deltaTime);
                    if (fadingModels[i].CurrentAlpha <= 0.0f)
                    {
                        fadingModels[i].Deinitialize();
                        fadingModels.RemoveAt(i);
                    }
                }
            }
           
            public void RegisterHandler(IUnitRendererHandler unitRendererHandler)
            {
                unitRendererHandlers.Add(unitRendererHandler);

                foreach (UnitRenderer unitRenderer in unitRenderers)
                    unitRendererHandler.HandleUnitRendererAttach(unitRenderer);
            }

            public void UnregisterHandler(IUnitRendererHandler unitRendererHandler)
            {
                foreach (UnitRenderer unitRenderer in unitRenderers)
                    unitRendererHandler.HandleUnitRendererDetach(unitRenderer);

                unitRendererHandlers.Remove(unitRendererHandler);
            }

            public bool TryFind(Unit unit, out UnitRenderer unitRenderer)
            {
                return TryFind(unit.Id, out unitRenderer);
            }

            public bool TryFind(ulong unitId, out UnitRenderer unitRenderer)
            {
                return unitRenderersById.TryGetValue(unitId, out unitRenderer);
            }

            private void AttachRenderer(Unit unit)
            {
                var unitRenderer = GameObjectPool.Take(unitRendererPrototype);
                unitRenderer.transform.SetParent(rendering.container);
                unitRenderer.Attach(unit);
                unitRenderersById.Add(unit.Id, unitRenderer);
                unitRenderers.Add(unitRenderer);

                rendering.selectionCircleController.HandleRendererAttach(unitRenderer);

                foreach (IUnitRendererHandler handler in unitRendererHandlers)
                    handler.HandleUnitRendererAttach(unitRenderer);
            }

            private void DetachRenderer(Unit unit)
            {
                if (unitRenderersById.TryGetValue(unit.Id, out UnitRenderer unitRenderer))
                {
                    rendering.spellVisualController.HandleRendererDetach(unitRenderer);
                    rendering.selectionCircleController.HandleRendererDetach(unitRenderer);

                    foreach (IUnitRendererHandler handler in unitRendererHandlers)
                        handler.HandleUnitRendererDetach(unitRenderer);

                    UnitModel detachedModel = unitRenderer.Detach(UnitModelReplacementMode.ScopeOut);
                    unitRenderersById.Remove(unit.Id);
                    unitRenderers.Remove(unitRenderer);

                    if (detachedModel != null)
                    {
                        detachedModel.transform.SetParent(rendering.container, true);
                        detachedModel.Animator.enabled = false;
                        detachedModel.ToggleTransparentMode(true, detachedModel.CurrentAlpha, 0.0f);
                        fadingModels.Add(detachedModel);
                    }

                    GameObjectPool.Return(unitRenderer, unitRenderer.gameObject == null);
                }
            }

            private void OnServerVisibilityChanged(WorldEntity worldEntity, bool visible)
            {
                var unit = worldEntity as Unit;
                if (unit == null)
                    return;

                if (visible && !unitRenderersById.ContainsKey(unit.Id))
                    AttachRenderer(unit);
                else if (!visible && unitRenderersById.ContainsKey(unit.Id))
                    DetachRenderer(unit);
            }

            private void OnEventEntityAttached(WorldEntity worldEntity)
            {
                if (worldEntity is Unit unit)
                {
                    AttachRenderer(unit);
                }
            }

            private void OnEventEntityDetach(WorldEntity worldEntity)
            {
                if (worldEntity is Unit unit)
                    DetachRenderer(unit);
            }
        }
    }
}
