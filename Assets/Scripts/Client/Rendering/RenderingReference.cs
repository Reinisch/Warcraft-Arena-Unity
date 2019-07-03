using System.Collections.Generic;
using Client.Spells;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using EventHandler = Common.EventHandler;

namespace Client
{
    [CreateAssetMenu(fileName = "Rendering Reference", menuName = "Game Data/Scriptable/Rendering", order = 1)]
    public partial class RenderingReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private PhotonBoltReference photon;
        [SerializeField, UsedImplicitly] private Sprite defaultSpellIcon;
        [SerializeField, UsedImplicitly] private NameplateController nameplateController;
        [SerializeField, UsedImplicitly] private FloatingTextController floatingTextController;
        [SerializeField, UsedImplicitly] private SpellVisualController spellVisualController;
        [SerializeField, UsedImplicitly] private SelectionCircleController selectionCircleController;
        [SerializeField, UsedImplicitly] private List<SpellVisualSettings> spellVisualSettings;

        private readonly Dictionary<int, SpellVisualSettings> spellVisualSettingsById = new Dictionary<int, SpellVisualSettings>();
        private readonly Dictionary<ulong, UnitRenderer> unitRenderersById = new Dictionary<ulong, UnitRenderer>();
        private readonly List<UnitRenderer> unitRenderers = new List<UnitRenderer>();
        private readonly List<IUnitRendererHandler> unitRendererHandlers = new List<IUnitRendererHandler>();

        public Player Player { get; set; }

        public Sprite DefaultSpellIcon => defaultSpellIcon;
        public IReadOnlyDictionary<int, SpellVisualSettings> SpellVisualSettingsById => spellVisualSettingsById;

        protected override void OnRegistered()
        {
            spellVisualSettings.ForEach(visual => spellVisualSettingsById.Add(visual.SpellInfo.Id, visual));
            spellVisualSettings.ForEach(visual => visual.Initialize());

            nameplateController.Initialize();
            floatingTextController.Initialize();
            spellVisualController.Initialize();
            selectionCircleController.Initialize();

            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.RegisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlGained, OnPlayerControlGained);
            EventHandler.UnregisterEvent<Player>(photon, GameEvents.PlayerControlLost, OnPlayerControlLost);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            nameplateController.Deinitialize();
            selectionCircleController.Deinitialize();
            floatingTextController.Deinitialize();
            spellVisualController.Deinitialize();

            spellVisualSettings.ForEach(visual => visual.Deinitialize());
            spellVisualSettingsById.Clear();
        }

        protected override void OnUpdate(float deltaTime)
        {
            foreach (var unitRenderer in unitRenderers)
                unitRenderer.DoUpdate(deltaTime);

            nameplateController.DoUpdate(deltaTime);
            floatingTextController.DoUpdate(deltaTime);
            spellVisualController.DoUpdate(deltaTime);
        }

        public bool TryFind(Unit unit, out UnitRenderer unitRenderer)
        {
            return unitRenderersById.TryGetValue(unit.Id, out unitRenderer);
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;

                EventHandler.RegisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.RegisterEvent<Unit, int, SpellProcessingToken>(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, OnSpellLaunch);
            }
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                EventHandler.UnregisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.UnregisterEvent<Unit, int, SpellProcessingToken>(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, OnSpellLaunch);

                worldManager.UnitManager.EventEntityAttached -= OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach -= OnEventEntityDetach;

                foreach (UnitRenderer unitRenderer in unitRenderers)
                    unitRenderer.Deinitialize();

                unitRenderersById.Clear();
                unitRenderers.Clear();
            }
        }

        private void OnPlayerControlGained(Player player)
        {
            Player = player;

            nameplateController.HandlePlayerControlGained();
        }

        private void OnPlayerControlLost(Player player)
        {
            nameplateController.HandlePlayerControlLost();

            Player = null;
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damageAmount, bool isCrit)
        {
            if (!caster.IsController)
                return;

            if (!unitRenderersById.TryGetValue(target.Id, out UnitRenderer targetRenderer))
                return;

            floatingTextController.SpawnDamageText(targetRenderer, damageAmount);
        }

        private void OnSpellLaunch(Unit caster, int spellId, SpellProcessingToken processingToken)
        {
            if (!unitRenderersById.TryGetValue(caster.Id, out UnitRenderer casterRenderer))
                return;

            casterRenderer.Animator.SetTrigger(AnimatorUtils.SpellCastAnimationTrigger);

            if (!SpellVisualSettingsById.TryGetValue(spellId, out SpellVisualSettings spellVisuals))
                return;

            if (processingToken != null && spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Projectile, out EffectSpellSettings settings))
                foreach (var entry in processingToken.ProcessingEntries)
                    if (unitRenderersById.TryGetValue(entry.Item1, out UnitRenderer targetRenderer))
                        spellVisualController.SpawnVisual(casterRenderer, targetRenderer, settings, processingToken.ServerFrame, entry.Item2);

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Cast, out EffectSpellSettings spellVisualEffect))
                spellVisualEffect.EffectSettings.PlayEffect(caster.Position, caster.Rotation)?.ApplyPositioning(casterRenderer.TagContainer, spellVisualEffect);
        }

        private void OnEventEntityAttached(WorldEntity worldEntity)
        {
            if (worldEntity is Unit unitEntity)
            {
                var unitRenderer = unitEntity.GetComponentInChildren<UnitRenderer>();
                unitRenderer.Initialize(unitEntity);
                unitRenderersById.Add(unitEntity.Id, unitRenderer);
                unitRenderers.Add(unitRenderer);

                selectionCircleController.HandleRendererAttach(unitRenderer);

                foreach (IUnitRendererHandler handler in unitRendererHandlers)
                    handler.HandleUnitRendererAttach(unitRenderer);
            }
        }

        private void OnEventEntityDetach(WorldEntity worldEntity)
        {
            if (worldEntity is Unit unitEntity && unitRenderersById.TryGetValue(unitEntity.Id, out UnitRenderer unitRenderer))
            {
                spellVisualController.HandleRendererDetach(unitRenderer);
                selectionCircleController.HandleRendererDetach(unitRenderer);

                foreach (IUnitRendererHandler handler in unitRendererHandlers)
                    handler.HandleUnitRendererDetach(unitRenderer);

                unitRenderer.Deinitialize();
                unitRenderersById.Remove(unitEntity.Id);
                unitRenderers.Remove(unitRenderer);
            }
        }

        private void RegisterHandler(IUnitRendererHandler unitRendererHandler)
        {
            unitRendererHandlers.Add(unitRendererHandler);

            foreach (UnitRenderer unitRenderer in unitRenderers)
                unitRendererHandler.HandleUnitRendererAttach(unitRenderer);
        }

        private void UnregisterHandler(IUnitRendererHandler unitRendererHandler)
        {
            foreach (UnitRenderer unitRenderer in unitRenderers)
                unitRendererHandler.HandleUnitRendererDetach(unitRenderer);

            unitRendererHandlers.Remove(unitRendererHandler);
        }
    }
}
