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
        private class NameplateController : IUnitRendererHandler
        {
            [SerializeField, UsedImplicitly] private RenderingReference rendering;
            [SerializeField, UsedImplicitly] private Nameplate nameplatePrototype;
            [SerializeField, UsedImplicitly] private NameplateSettings settings;
            [SerializeField, UsedImplicitly] private GameOptionBool showDeselectedHealthOption;
            [SerializeField, UsedImplicitly] private int preinstantiatedCount = 20;

            private readonly List<Nameplate> activeNameplates = new List<Nameplate>();
            private readonly List<UnitRenderer> unplatedRenderers = new List<UnitRenderer>();
            private readonly Dictionary<UnitRenderer, Nameplate> activeNameplateByRenderers = new Dictionary<UnitRenderer, Nameplate>();

            public void Initialize()
            {
                GameObjectPool.PreInstantiate(nameplatePrototype.gameObject, preinstantiatedCount);

                EventHandler.RegisterEvent(showDeselectedHealthOption, GameEvents.GameOptionChanged, OnHealthOptionChanged);
            }

            public void Deinitialize()
            {
                EventHandler.UnregisterEvent(showDeselectedHealthOption, GameEvents.GameOptionChanged, OnHealthOptionChanged);

                for (int i = activeNameplates.Count - 1; i >= 0; i--)
                {
                    GameObjectPool.Return(activeNameplates[i], true);
                    Destroy(activeNameplates[i]);
                }

                activeNameplates.Clear();
                activeNameplateByRenderers.Clear();
                unplatedRenderers.Clear();
            }

            public void HandleUnitRendererAttach(UnitRenderer attachedRenderer)
            {
                if (CanHaveNameplate(attachedRenderer))
                    SpawnNameplate(attachedRenderer);
                else
                    unplatedRenderers.Add(attachedRenderer);
            }

            public void HandleUnitRendererDetach(UnitRenderer detachedRenderer)
            {
                unplatedRenderers.Remove(detachedRenderer);

                if (activeNameplateByRenderers.TryGetValue(detachedRenderer, out Nameplate nameplate))
                    DespawnNameplate(nameplate);
            }

            public void HandlePlayerControlGained()
            {
                EventHandler.RegisterEvent(rendering.Player, GameEvents.UnitTargetChanged, OnPlayerTargetChanged);

                rendering.RegisterHandler(this);
            }

            public void HandlePlayerControlLost()
            {
                rendering.UnregisterHandler(this);

                EventHandler.UnregisterEvent(rendering.Player, GameEvents.UnitTargetChanged, OnPlayerTargetChanged);
            }

            public void DoUpdate(float deltaTime)
            {
                for (int i = unplatedRenderers.Count - 1; i >= 0; i--)
                {
                    if (CanHaveNameplate(unplatedRenderers[i]))
                    {
                        SpawnNameplate(unplatedRenderers[i]);
                        unplatedRenderers.RemoveAt(i);
                    }
                }

                for (int i = activeNameplates.Count - 1; i >= 0; i--)
                {
                    if (!activeNameplates[i].DoUpdate(deltaTime))
                    {
                        unplatedRenderers.Add(activeNameplates[i].UnitRenderer);
                        DespawnNameplate(activeNameplates[i]);
                    }
                }
            }

            private void SpawnNameplate(UnitRenderer targetRenderer)
            {
                Nameplate newNameplate = GameObjectPool.Take(nameplatePrototype);
                newNameplate.UpdateUnit(targetRenderer);

                activeNameplates.Add(newNameplate);
                activeNameplateByRenderers.Add(targetRenderer, newNameplate);
            }

            private void DespawnNameplate(Nameplate nameplate)
            {
                activeNameplateByRenderers.Remove(nameplate.UnitRenderer);
                activeNameplates.Remove(nameplate);

                nameplate.UpdateUnit(null);
                GameObjectPool.Return(nameplate, false);
            }

            private bool CanHaveNameplate(UnitRenderer unitRenderer)
            {
                bool withinRange = rendering.Player.ExactDistanceSqrTo(unitRenderer.Unit) < settings.MaxDistanceSqr;
                bool isTransparent = unitRenderer.Unit.VisualEffects.HasTargetFlag(UnitVisualEffectFlags.AnyTransparency);
                return withinRange && !isTransparent;
            }

            private void OnPlayerTargetChanged()
            {
                activeNameplates.ForEach(nameplate => nameplate.UpdateSelection());
            }

            private void OnHealthOptionChanged()
            {
                for (int i = activeNameplates.Count - 1; i >= 0; i--)
                    activeNameplates[i].UpdateSelection();
            }
        }
    }
}
