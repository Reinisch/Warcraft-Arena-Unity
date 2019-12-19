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
    public partial class RenderingReference : ScriptableReferenceClient
    { 
        [SerializeField, UsedImplicitly] private Sprite defaultSpellIcon;
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private UnitModelSettingsContainer modelSettingsContainer;
        [SerializeField, UsedImplicitly] private UnitRendererSettings unitRendererSettings;
        [Header("Controllers")]
        [SerializeField, UsedImplicitly] private NameplateController nameplateController;
        [SerializeField, UsedImplicitly] private FloatingTextController floatingTextController;
        [SerializeField, UsedImplicitly] private SpellVisualController spellVisualController;
        [SerializeField, UsedImplicitly] private SelectionCircleController selectionCircleController;
        [SerializeField, UsedImplicitly] private UnitRendererController unitRendererController;
        [Header("Collections")]
        [SerializeField, UsedImplicitly] private SpellVisualsInfoContainer spellVisualsInfoContainer;
        [SerializeField, UsedImplicitly] private List<AuraEffectSettings> auraEffectSettings;
        [SerializeField, UsedImplicitly] private ClassTypeSpriteDictionary classIconsByClassType;
        [SerializeField, UsedImplicitly] private SpellPowerTypeColorDictionary colorsBySpellPowerType;
        [SerializeField, UsedImplicitly] private List<Material> autoIncludedMaterials;

        private readonly Dictionary<int, AuraEffectSettings> auraVisualSettingsById = new Dictionary<int, AuraEffectSettings>();
        
        private Transform container;

        public Sprite DefaultSpellIcon => defaultSpellIcon;
        public UnitRendererSettings UnitRendererSettings => unitRendererSettings;
        public IReadOnlyDictionary<int, SpellVisualsInfo> SpellVisuals => spellVisualsInfoContainer.SpellVisualsInfosById;
        public IReadOnlyDictionary<int, AuraEffectSettings> AuraVisualSettingsById => auraVisualSettingsById;
        public IReadOnlyDictionary<int, UnitModelSettings> ModelSettingsById => modelSettingsContainer.ModelSettingsById;
        public IReadOnlySerializedDictionary<ClassType, Sprite> ClassIconsByClassType => classIconsByClassType;
        public IReadOnlySerializedDictionary<SpellPowerType, Color> ColorsBySpellPowerType => colorsBySpellPowerType;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            container = GameObject.FindGameObjectWithTag("Renderer Container").transform;

            classIconsByClassType.Register();
            colorsBySpellPowerType.Register();
            modelSettingsContainer.Register();
            auraEffectSettings.ForEach(visual => auraVisualSettingsById.Add(visual.AuraInfo.Id, visual));
            spellVisualsInfoContainer.Register();
        }

        protected override void OnUnregister()
        {
            spellVisualsInfoContainer.Unregister();
            auraVisualSettingsById.Clear();
            classIconsByClassType.Unregister();
            colorsBySpellPowerType.Unregister();
            modelSettingsContainer.Unregister();

            container = null;

            base.OnUnregister();
        }

        protected override void OnUpdate(float deltaTime)
        {
            unitRendererController.DoUpdate(deltaTime);
            nameplateController.DoUpdate(deltaTime);
            floatingTextController.DoUpdate(deltaTime);
            spellVisualController.DoUpdate(deltaTime);
        }
        
        protected override void OnWorldStateChanged(World world, bool created)
        {
            if (created)
            {
                base.OnWorldStateChanged(world, true);

                EventHandler.RegisterEvent<Unit, Unit, int, HitType>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.RegisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellHealingDone, OnSpellHealingDone);
                EventHandler.RegisterEvent<Unit, Unit, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.SpellMissDone, OnSpellMiss);
                EventHandler.RegisterEvent<Unit, int, SpellProcessingToken>(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.RegisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellHit, OnSpellHit);

                nameplateController.Initialize();
                floatingTextController.Initialize();
                spellVisualController.Initialize();
                selectionCircleController.Initialize();
                unitRendererController.Initialize();
            }
            else
            {
                unitRendererController.Deinitialize();
                nameplateController.Deinitialize();
                selectionCircleController.Deinitialize();
                floatingTextController.Deinitialize();
                spellVisualController.Deinitialize();

                EventHandler.UnregisterEvent<Unit, Unit, int, HitType>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.UnregisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellHealingDone, OnSpellHealingDone);
                EventHandler.UnregisterEvent<Unit, Unit, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.SpellMissDone, OnSpellMiss);
                EventHandler.UnregisterEvent<Unit, int, SpellProcessingToken>(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.UnregisterEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellHit, OnSpellHit);

                base.OnWorldStateChanged(world, false);
            }
        }

        protected override void OnControlStateChanged(Player player, bool underControl)
        {
            if (underControl)
            {
                base.OnControlStateChanged(player, true);

                nameplateController.HandlePlayerControlGained();
                selectionCircleController.HandlePlayerControlGained();
                unitRendererController.UpdateClientsideVisibility();
            }
            else
            {
                unitRendererController.UpdateClientsideVisibility();
                nameplateController.HandlePlayerControlLost();
                selectionCircleController.HandlePlayerControlLost();

                base.OnControlStateChanged(player, false);
            }
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damageAmount, HitType hitType)
        {
            if (!caster.IsController)
                return;

            if (unitRendererController.TryFind(target, out UnitRenderer targetRenderer))
                floatingTextController.SpawnDamageText(targetRenderer, damageAmount, hitType);
        }

        private void OnSpellMiss(Unit caster, Unit target, SpellMissType missType)
        {
            if (!caster.IsController)
                return;

            if (!unitRendererController.TryFind(target.Id, out UnitRenderer targetRenderer))
                return;

            floatingTextController.SpawnMissText(targetRenderer, missType);
        }

        private void OnSpellHealingDone(Unit caster, Unit target, int healingAmount, bool isCrit)
        {
            if (!caster.IsController)
                return;

            if (!unitRendererController.TryFind(target.Id, out UnitRenderer targetRenderer))
                return;

            floatingTextController.SpawnHealingText(targetRenderer, healingAmount, isCrit);
        }

        private void OnSpellLaunch(Unit caster, int spellId, SpellProcessingToken processingToken)
        {
            if (!balance.SpellInfosById.TryGetValue(spellId, out SpellInfo spellInfo))
                return;

            if (!unitRendererController.TryFind(caster.Id, out UnitRenderer casterRenderer))
                return;

            if (!spellInfo.HasAttribute(SpellCustomAttributes.CastWithoutAnimation))
                casterRenderer.TriggerInstantCast();

            if (!SpellVisuals.TryGetValue(spellId, out SpellVisualsInfo spellVisuals))
                return;

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Projectile, out EffectSpellSettings settings))
                foreach (var entry in processingToken.ProcessingEntries)
                    if (unitRendererController.TryFind(entry.Item1, out UnitRenderer targetRenderer))
                        spellVisualController.SpawnVisual(casterRenderer, targetRenderer, settings, processingToken.ServerFrame, entry.Item2);

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Cast, out EffectSpellSettings spellVisualEffect))
            {
                IEffectEntity effectEntity = spellVisualEffect.EffectSettings.PlayEffect(processingToken.Source + Vector3.up, caster.Rotation);
                if (effectEntity != null && !spellInfo.HasAttribute(SpellCustomAttributes.LaunchSourceIsExplicit))
                    effectEntity.ApplyPositioning(casterRenderer.TagContainer, spellVisualEffect);
            }

            if (spellInfo.ExplicitTargetType == SpellExplicitTargetType.Destination)
                if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Destination, out EffectSpellSettings destinationEffect))
                    destinationEffect.EffectSettings.PlayEffect(processingToken.Destination + Vector3.up, caster.Rotation);
        }

        private void OnSpellHit(Unit target, int spellId)
        {
            if (!unitRendererController.TryFind(target.Id, out UnitRenderer targetRenderer))
                return;

            if (!SpellVisuals.TryGetValue(spellId, out SpellVisualsInfo spellVisuals))
                return;

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Impact, out EffectSpellSettings spellVisualEffect))
            {
                IEffectEntity effectEntity = spellVisualEffect.EffectSettings.PlayEffect(target.Position, target.Rotation);
                effectEntity?.ApplyPositioning(targetRenderer.TagContainer, spellVisualEffect);
            }
        }

        private void RegisterHandler(IUnitRendererHandler unitRendererHandler) => unitRendererController.RegisterHandler(unitRendererHandler);

        private void UnregisterHandler(IUnitRendererHandler unitRendererHandler) => unitRendererController.UnregisterHandler(unitRendererHandler);

#if UNITY_EDITOR
        [ContextMenu("Collect Aura Effects"), UsedImplicitly]
        private void CollectAuraEffectSettings()
        {
            auraEffectSettings.Clear();

            foreach (string guid in UnityEditor.AssetDatabase.FindAssets($"t:{nameof(AuraEffectSettings)}", null))
                auraEffectSettings.Add(UnityEditor.AssetDatabase.LoadAssetAtPath<AuraEffectSettings>(UnityEditor.AssetDatabase.GUIDToAssetPath(guid)));
        }

        [ContextMenu("Collect Everything"), UsedImplicitly]
        private void CollectEverything()
        {
            CollectAuraEffectSettings();
        }
#endif
    }
}
