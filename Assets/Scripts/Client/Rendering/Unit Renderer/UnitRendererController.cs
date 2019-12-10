using System;
using System.Collections.Generic;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

using EventHandler = Common.EventHandler;

namespace Client
{
    public partial class RenderingReference
    {
        [Serializable]
        private class UnitRendererController
        {
            [SerializeField, UsedImplicitly] private RenderingReference rendering;
            [SerializeField, UsedImplicitly] private UnitRenderer unitRendererPrototype;

            private readonly Dictionary<ulong, UnitRenderer> unitRenderersById = new Dictionary<ulong, UnitRenderer>();
            private readonly List<UnitRenderer> unitRenderers = new List<UnitRenderer>();
            private readonly List<UnitModel> fadingModels = new List<UnitModel>();
            private readonly HashSet<Unit> detachedUnits = new HashSet<Unit>();
            private readonly List<IUnitRendererHandler> unitRendererHandlers = new List<IUnitRendererHandler>();

            public void Initialize()
            {
                Assert.IsTrue(unitRendererHandlers.Count == 0);
                Assert.IsTrue(unitRenderers.Count == 0);
                Assert.IsTrue(unitRenderersById.Count == 0);
                Assert.IsTrue(detachedUnits.Count == 0);

                rendering.World.UnitManager.EventEntityAttached += OnEventEntityAttached;
                rendering.World.UnitManager.EventEntityDetach += OnEventEntityDetach;
                EventHandler.RegisterEvent<WorldEntity, bool>(rendering.World, GameEvents.ServerVisibilityChanged, OnServerVisibilityChanged);
            }

            public void Deinitialize()
            {
                EventHandler.UnregisterEvent<WorldEntity, bool>(rendering.World, GameEvents.ServerVisibilityChanged, OnServerVisibilityChanged);
                rendering.World.UnitManager.EventEntityAttached -= OnEventEntityAttached;
                rendering.World.UnitManager.EventEntityDetach -= OnEventEntityDetach;

                foreach (UnitRenderer unitRenderer in unitRenderers)
                    unitRenderer.Detach(UnitModelReplacementMode.Complete);

                foreach (UnitModel fadingModel in fadingModels)
                    fadingModel.Deinitialize();

                unitRenderersById.Clear();
                unitRenderers.Clear();
                detachedUnits.Clear();
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

            public void UpdateClientsideVisibility()
            {
                for (int i = unitRenderers.Count - 1; i >= 0; i--)
                    if (!IsVisibleForPlayer(unitRenderers[i].Unit))
                        DetachRenderer(unitRenderers[i].Unit);

                List<Unit> newVisibleUnits = ListPoolContainer<Unit>.Take();
                foreach(Unit detachedUnit in detachedUnits)
                    if (IsVisibleForPlayer(detachedUnit))
                        newVisibleUnits.Add(detachedUnit);

                foreach (Unit newVisibleUnit in newVisibleUnits)
                    AttachRenderer(newVisibleUnit);

                ListPoolContainer<Unit>.Return(newVisibleUnits);
            }

            private bool IsVisibleForPlayer(WorldEntity target) => rendering.Player != null && rendering.Player.HasClientVisiblityOf(target);

            private void AttachRenderer(Unit unit)
            {
                var unitRenderer = GameObjectPool.Take(unitRendererPrototype);
                unitRenderer.transform.SetParent(rendering.container);
                unitRenderer.Attach(unit);
                unitRenderersById.Add(unit.Id, unitRenderer);
                unitRenderers.Add(unitRenderer);
                detachedUnits.Remove(unit);

                rendering.selectionCircleController.HandleRendererAttach(unitRenderer);

                foreach (IUnitRendererHandler handler in unitRendererHandlers)
                    handler.HandleUnitRendererAttach(unitRenderer);
            }

            private void DetachRenderer(Unit unit)
            {
                detachedUnits.Remove(unit);

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
                    if (IsVisibleForPlayer(unit))
                        AttachRenderer(unit);
                    else
                        detachedUnits.Add(unit);
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
