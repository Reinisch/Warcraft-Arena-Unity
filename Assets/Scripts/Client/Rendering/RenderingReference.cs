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
        [SerializeField, UsedImplicitly] private AuraVisualsInfoContainer auraVisualsInfoContainer;
        [SerializeField, UsedImplicitly] private AnimationInfoContainer animationInfoContainer;
        [SerializeField, UsedImplicitly] private SpellAnimationInfoContainer spellAnimationInfoContainer;
        [SerializeField, UsedImplicitly] private ClassTypeSpriteDictionary classIconsByClassType;
        [SerializeField, UsedImplicitly] private SpellPowerTypeColorDictionary colorsBySpellPowerType;
        [SerializeField, UsedImplicitly] private List<Material> autoIncludedMaterials;

        private Transform container;
        private readonly Dictionary<Collider, UnitRenderer> unitRenderersByHitBoxes = new Dictionary<Collider, UnitRenderer>();

        public Sprite DefaultSpellIcon => defaultSpellIcon;
        public UnitRendererSettings UnitRendererSettings => unitRendererSettings;
        public IReadOnlyDictionary<int, SpellVisualsInfo> SpellVisuals => spellVisualsInfoContainer.SpellVisualsInfosById;
        public IReadOnlyDictionary<int, AuraVisualsInfo> AuraVisuals => auraVisualsInfoContainer.AuraVisualsInfosById;
        public IReadOnlyDictionary<int, UnitModelSettings> Models => modelSettingsContainer.ModelSettingsById;
        public IReadOnlySerializedDictionary<ClassType, Sprite> ClassIconSprites => classIconsByClassType;
        public IReadOnlySerializedDictionary<SpellPowerType, Color> SpellPowerColors => colorsBySpellPowerType;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            if (!Application.isEditor)
                autoIncludedMaterials.Clear();

            container = GameObject.FindGameObjectWithTag("Renderer Container").transform;

            classIconsByClassType.Register();
            colorsBySpellPowerType.Register();
            modelSettingsContainer.Register();
            auraVisualsInfoContainer.Register();
            spellVisualsInfoContainer.Register();
            animationInfoContainer.Register();
            spellAnimationInfoContainer.Register();

            EventHandler.RegisterEvent<UnitModel, UnitRenderer, bool>(this, GameEvents.UnitModelAttached, OnUnitModelAttached);
        }

        protected override void OnUnregister()
        {
            EventHandler.UnregisterEvent<UnitModel, UnitRenderer, bool>(this, GameEvents.UnitModelAttached, OnUnitModelAttached);

            spellAnimationInfoContainer.Unregister();
            animationInfoContainer.Unregister();
            spellVisualsInfoContainer.Unregister();
            auraVisualsInfoContainer.Unregister();
            classIconsByClassType.Unregister();
            colorsBySpellPowerType.Unregister();
            modelSettingsContainer.Unregister();

            Assert.IsTrue(unitRenderersByHitBoxes.Count == 0);

            unitRenderersByHitBoxes.Clear();
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

                EventHandler.RegisterEvent<Unit, Unit, int, HitType>(GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.RegisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnSpellHealingDone);
                EventHandler.RegisterEvent<Unit, Unit, SpellMissType>(GameEvents.SpellMissDone, OnSpellMiss);
                EventHandler.RegisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.RegisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);

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

                EventHandler.UnregisterEvent<Unit, Unit, int, HitType>(GameEvents.SpellDamageDone, OnSpellDamageDone);
                EventHandler.UnregisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnSpellHealingDone);
                EventHandler.UnregisterEvent<Unit, Unit, SpellMissType>(GameEvents.SpellMissDone, OnSpellMiss);
                EventHandler.UnregisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
                EventHandler.UnregisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);

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
                casterRenderer.TriggerInstantCast(spellInfo);

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

        private void OnUnitModelAttached(UnitModel unitModel, UnitRenderer unitRenderer, bool isAttached)
        {
            for (int i = 0; i < unitModel.HitBoxes.Count; i++)
            {
                if (isAttached)
                    unitRenderersByHitBoxes.Add(unitModel.HitBoxes[i], unitRenderer);
                else
                    unitRenderersByHitBoxes.Remove(unitModel.HitBoxes[i]);
            }
        }

        private void RegisterHandler(IUnitRendererHandler unitRendererHandler) => unitRendererController.RegisterHandler(unitRendererHandler);

        private void UnregisterHandler(IUnitRendererHandler unitRendererHandler) => unitRendererController.UnregisterHandler(unitRendererHandler);

        public AnimationInfo FindAnimation(SpellInfo spellInfo) => spellAnimationInfoContainer.FindAnimation(spellInfo);

        public bool TryFindRenderer(Collider hitBox, out UnitRenderer unitRenderer) => unitRenderersByHitBoxes.TryGetValue(hitBox, out unitRenderer);
    }
}
