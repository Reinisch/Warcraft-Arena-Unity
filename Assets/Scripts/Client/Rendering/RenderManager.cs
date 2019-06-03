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

            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.RegisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);
        }

        public new void Deinitialize()
        {
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldInitialized, OnWorldInitialized);
            EventHandler.UnregisterEvent<WorldManager>(EventHandler.GlobalDispatcher, GameEvents.WorldDeinitializing, OnWorldDeinitializing);

            spellVisualSettings.ForEach(visual => visual.Deinitialize());
            spellVisualSettingsById.Clear();

            base.Deinitialize();
        }

        public void DoUpdate(int deltaTime)
        {
            foreach (var unitEntry in unitRenderers)
                unitEntry.Value.DoUpdate(deltaTime);
        }

        private void OnWorldInitialized(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;

                SpellManager.Instance.EventSpellCast += OnSpellCast;
                SpellManager.Instance.EventSpellDamageDone += OnSpellDamageDone;
            }
        }

        private void OnWorldDeinitializing(WorldManager worldManager)
        {
            if (worldManager.HasClientLogic)
            {
                SpellManager.Instance.EventSpellDamageDone -= OnSpellDamageDone;
                SpellManager.Instance.EventSpellCast -= OnSpellCast;

                worldManager.UnitManager.EventEntityAttached -= OnEventEntityAttached;
                worldManager.UnitManager.EventEntityDetach -= OnEventEntityDetach;

                foreach (var unitRendererRecord in unitRenderers)
                    unitRendererRecord.Value.Deinitialize();

                unitRenderers.Clear();
            }
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damage, bool isCrit)
        {
            if (!unitRenderers.ContainsKey(target))
                return;

            if (caster.IsController)
            {
                GameObject damageEvent = Instantiate(Resources.Load("Prefabs/UI/DamageEvent")) as GameObject;
                Assert.IsNotNull(damageEvent, "damageEvent != null");
                // damageEvent.GetComponent<UnitDamageUIEvent>().Initialize(damage, unitRenderers[caster], isCrit, ArenaManager.PlayerInterface);
            }
        }

        private void OnSpellCast(Unit caster, SpellInfo spellInfo)
        {
            if (!SpellVisualSettingsById.TryGetValue(spellInfo.Id, out SpellVisualSettings spellVisuals))
                return;

            if (!spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Cast, out EffectSpellSettings spellVisualEffect))
                return;

            if (unitRenderers.TryGetValue(caster, out UnitRenderer casterRenderer))
                spellVisualEffect.EffectSettings.PlayEffect(caster.Position, caster.Rotation)?.ApplyPositioning(casterRenderer.EffectTagPositioner, spellVisualEffect);
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