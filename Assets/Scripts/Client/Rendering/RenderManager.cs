using System.Collections.Generic;
using Client.Spells;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class RenderManager : SingletonBehaviour<RenderManager>
    {
        [SerializeField, UsedImplicitly] private Sprite defaultSpellIcon;
        [SerializeField, UsedImplicitly] private FloatingTextController floatingTextController;
        [SerializeField, UsedImplicitly] private List<SpellVisualSettings> spellVisualSettings;

        private readonly Dictionary<int, SpellVisualSettings> spellVisualSettingsById = new Dictionary<int, SpellVisualSettings>();
        private readonly Dictionary<Unit, UnitRenderer> unitRenderers = new Dictionary<Unit, UnitRenderer>();

        public Sprite DefaultSpellIcon => defaultSpellIcon;
        public IReadOnlyDictionary<int, SpellVisualSettings> SpellVisualSettingsById => spellVisualSettingsById;

        public new void Initialize()
        {
            base.Initialize();

            spellVisualSettings.ForEach(visual => spellVisualSettingsById.Add(visual.SpellInfo.Id, visual));
            spellVisualSettings.ForEach(visual => visual.Initialize());
            floatingTextController.Initialize();

            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        public new void Deinitialize()
        {
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            floatingTextController.Deinitialize();
            spellVisualSettings.ForEach(visual => visual.Deinitialize());
            spellVisualSettingsById.Clear();

            base.Deinitialize();
        }

        public void DoUpdate(float deltaTime)
        {
            foreach (var unitEntry in unitRenderers)
                unitEntry.Value.DoUpdate(deltaTime);

            floatingTextController.DoUpdate(deltaTime);
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;

                EventHandler.RegisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.RegisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellCasted, OnSpellCast);
            }
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                EventHandler.UnregisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.UnregisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellCasted, OnSpellCast);

                worldManager.UnitManager.EventEntityAttached -= OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach -= OnEventEntityDetach;

                foreach (var unitRendererRecord in unitRenderers)
                    unitRendererRecord.Value.Deinitialize();

                unitRenderers.Clear();
            }
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damageAmount, bool isCrit)
        {
            if (!caster.IsController)
                return;

            if (!unitRenderers.TryGetValue(target, out UnitRenderer targetRenderer))
                return;

            floatingTextController.SpawnDamageText(targetRenderer, damageAmount);
        }

        private void OnSpellCast(Unit caster, int spellId)
        {
            if (!unitRenderers.TryGetValue(caster, out UnitRenderer casterRenderer))
                return;

            casterRenderer.Animator.SetTrigger(AnimatorUtils.SpellCastAnimationTrigger);

            if (!BalanceManager.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            if (!SpellVisualSettingsById.TryGetValue(spellInfo.Id, out SpellVisualSettings spellVisuals))
                return;

            if (!spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Cast, out EffectSpellSettings spellVisualEffect))
                return;

            spellVisualEffect.EffectSettings.PlayEffect(caster.Position, caster.Rotation)?.ApplyPositioning(casterRenderer.TagContainer, spellVisualEffect);
        }

        private void OnEventEntityAttached(WorldEntity worldEntity)
        {
            if (worldEntity is Unit unitEntity)
            {
                var unitRenderer = unitEntity.GetComponentInChildren<UnitRenderer>();
                unitRenderer.Initialize(unitEntity);
                unitRenderers.Add(unitEntity, unitRenderer);
            }
        }

        private void OnEventEntityDetach(WorldEntity worldEntity)
        {
            if (worldEntity is Unit unitEntity && unitRenderers.TryGetValue(unitEntity, out UnitRenderer unitRenderer))
            {
                unitRenderer.Deinitialize();
                unitRenderers.Remove(unitEntity);
            }
        }
    }
}