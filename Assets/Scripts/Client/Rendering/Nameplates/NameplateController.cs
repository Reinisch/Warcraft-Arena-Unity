using System;
using System.Collections.Generic;
using Common;
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
            [SerializeField, UsedImplicitly] private GameOptionBool showDeselectedHealthOption;
            [SerializeField, UsedImplicitly] private int preinstantiatedCount = 20;

            private readonly List<Nameplate> activeNameplates = new List<Nameplate>();
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
            }

            public void HandleUnitRendererAttach(UnitRenderer attachedRenderer)
            {
                SpawnNameplate(attachedRenderer);
            }

            public void HandleUnitRendererDetach(UnitRenderer detachedRenderer)
            {
                if (activeNameplateByRenderers.TryGetValue(detachedRenderer, out Nameplate nameplate))
                {
                    activeNameplateByRenderers.Remove(detachedRenderer);
                    activeNameplates.Remove(nameplate);

                    GameObjectPool.Return(nameplate, false);
                }
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

            public void DoUpdate()
            {
                for (int i = activeNameplates.Count - 1; i >= 0; i--)
                    activeNameplates[i].DoUpdate();
            }

            private void SpawnNameplate(UnitRenderer targetRenderer)
            {
                Nameplate newNameplate = GameObjectPool.Take(nameplatePrototype, targetRenderer.transform.position, targetRenderer.transform.rotation);
                targetRenderer.TagContainer.ApplyPositioning(newNameplate);
                newNameplate.UpdateUnit(targetRenderer);

                activeNameplates.Add(newNameplate);
                activeNameplateByRenderers.Add(targetRenderer, newNameplate);
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
