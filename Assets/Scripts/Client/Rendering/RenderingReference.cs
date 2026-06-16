using System.Collections.Generic;
using Client.Spells;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public partial class RenderingReference : ScriptableReferenceClient
    {
        [Inject] private BalanceReference balance;
        [Inject] private EventBus eventBus;
        [SerializeField, UsedImplicitly] private Sprite defaultSpellIcon;
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
        private readonly Dictionary<Collider, UnitRenderer> unitRenderersByHitBoxes = new();
        private readonly Dictionary<Collider, UnitProjectileHitBox> unitProjectileHitBoxesByCollider = new();
        private readonly Dictionary<Collider, UnitRenderer> unitRendererProjectileHitBoxesByCollider = new();

        // Lookups relocated off the ScriptableObject containers (their runtime state leaked between
        // editor/MPPM play sessions). Built from the containers' item lists in OnRegistered.
        private readonly Dictionary<int, SpellVisualsInfo> spellVisualsInfosById = new();
        private readonly Dictionary<int, AuraVisualsInfo> auraVisualsInfosById = new();
        private readonly Dictionary<int, UnitModelSettings> modelSettingsById = new();
        private readonly Dictionary<int, AnimationInfo> animationInfoBySpellId = new();

        public Sprite DefaultSpellIcon => defaultSpellIcon;
        public UnitRendererSettings UnitRendererSettings => unitRendererSettings;
        public IReadOnlyDictionary<int, SpellVisualsInfo> SpellVisuals => spellVisualsInfosById;
        public IReadOnlyDictionary<int, AuraVisualsInfo> AuraVisuals => auraVisualsInfosById;
        public IReadOnlyDictionary<int, UnitModelSettings> Models => modelSettingsById;
        public IReadOnlySerializedDictionary<ClassType, Sprite> ClassIconSprites => classIconsByClassType;
        public IReadOnlySerializedDictionary<SpellPowerType, Color> SpellPowerColors => colorsBySpellPowerType;
        public UnitRendererController UnitRenderers => unitRendererController;

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

            foreach (UnitModelSettings model in modelSettingsContainer.ItemList)
                modelSettingsById.Add(model.Id, model);

            foreach (AuraVisualsInfo auraVisual in auraVisualsInfoContainer.ItemList)
                auraVisualsInfosById.Add(auraVisual.AuraInfo.Id, auraVisual);

            foreach (SpellVisualsInfo spellVisual in spellVisualsInfoContainer.ItemList)
            {
                spellVisualsInfosById.Add(spellVisual.SpellInfo.Id, spellVisual);
                spellVisual.Initialize();
            }

            foreach (SpellAnimationInfo spellAnimation in spellAnimationInfoContainer.ItemList)
                animationInfoBySpellId.Add(spellAnimation.Spell.Id, spellAnimation.Animation);

            eventBus.RegisterEvent<UnitModel, UnitRenderer, bool>(this, GameEvents.UnitModelAttached, OnUnitModelAttached);
        }

        protected override void OnUnregister()
        {
            eventBus.UnregisterEvent<UnitModel, UnitRenderer, bool>(this, GameEvents.UnitModelAttached, OnUnitModelAttached);

            foreach (SpellVisualsInfo spellVisual in spellVisualsInfoContainer.ItemList)
                spellVisual.Deinitialize();

            animationInfoBySpellId.Clear();
            spellVisualsInfosById.Clear();
            auraVisualsInfosById.Clear();
            modelSettingsById.Clear();

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

        public override void OnWorldStateChanged(bool created)
        {
            if (created)
            {
                base.OnWorldStateChanged(true);

                eventBus.RegisterEvent<Unit, Unit, int, HitType, Vector3?>(GameEvents.SpellDamageDone, OnSpellDamageDone);
                eventBus.RegisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnSpellHealingDone);
                eventBus.RegisterEvent<Unit, Unit, SpellMissType>(GameEvents.SpellMissDone, OnSpellMiss);
                eventBus.RegisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
                eventBus.RegisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);

                nameplateController.Initialize(this);
                floatingTextController.Initialize();
                spellVisualController.Initialize();
                selectionCircleController.Initialize(this);
                unitRendererController.Initialize(this);
            }
            else
            {
                unitRendererController.Deinitialize();
                nameplateController.Deinitialize();
                selectionCircleController.Deinitialize();
                floatingTextController.Deinitialize();
                spellVisualController.Deinitialize();

                eventBus.UnregisterEvent<Unit, Unit, int, HitType, Vector3?>(GameEvents.SpellDamageDone, OnSpellDamageDone);
                eventBus.UnregisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnSpellHealingDone);
                eventBus.UnregisterEvent<Unit, Unit, SpellMissType>(GameEvents.SpellMissDone, OnSpellMiss);
                eventBus.UnregisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnSpellLaunch);
                eventBus.UnregisterEvent<Unit, int>(GameEvents.SpellHit, OnSpellHit);

                base.OnWorldStateChanged(false);
            }
        }

        public override void OnControlStateChanged(bool underControl)
        {
            if (underControl)
            {
                base.OnControlStateChanged(true);

                nameplateController.HandlePlayerControlGained();
                selectionCircleController.HandlePlayerControlGained();
            }
            else
            {
                nameplateController.HandlePlayerControlLost();
                selectionCircleController.HandlePlayerControlLost();

                base.OnControlStateChanged(false);
            }
        }

        protected override void QueueForInject(DiContainer container)
        {
            container.QueueForInject(unitRendererController);
            container.QueueForInject(nameplateController);
            container.QueueForInject(floatingTextController);
            container.QueueForInject(spellVisualController);

            modelSettingsContainer.QueueForInject(container);
            spellVisualsInfoContainer.QueueForInject(container);
            auraVisualsInfoContainer.QueueForInject(container);
            animationInfoContainer.QueueForInject(container);
            spellAnimationInfoContainer.QueueForInject(container);
        }

        public void OnProjectileImpact(Projectile projectile)
        {
            if (!SpellVisuals.TryGetValue(projectile.HitSpell.Id, out SpellVisualsInfo spellVisuals))
                return;

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Impact, out EffectSpellSettings spellVisualEffect))
            {
                EffectHandle handle = spellVisualEffect.EffectSettings.PlayEffect(projectile.Transform.position, projectile.Transform.rotation);
                if (handle.IsValid)
                {
                    handle.Entity.KeepAliveWithNoParticles = spellVisualEffect.KeepAliveWithNoParticles;
                    handle.Entity.KeepOriginalRotation = spellVisualEffect.KeepOriginalRotation;
                }
            }
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damageAmount, HitType hitType, Vector3? hitPosition)
        {
            if (!caster.IsController)
                return;

            if (unitRendererController.TryFind(target, out UnitRenderer targetRenderer))
                floatingTextController.SpawnDamageText(targetRenderer, damageAmount, hitType, hitPosition);
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

            bool sourceIsExplicit = spellInfo.HasAttribute(SpellCustomAttributes.LaunchSourceIsExplicit);

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Projectile, out EffectSpellSettings settings))
                foreach (var entry in processingToken.ProcessingEntries)
                    if (unitRendererController.TryFind(entry.Item1, out UnitRenderer targetRenderer))
                        spellVisualController.SpawnVisual(casterRenderer, processingToken.Source, targetRenderer, settings, entry.Item2, sourceIsExplicit);

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Cast, out EffectSpellSettings spellVisualEffect))
            {
                EffectHandle handle = spellVisualEffect.EffectSettings.PlayEffect(processingToken.Source + Vector3.up, caster.Rotation);
                if (handle.IsValid && !spellInfo.HasAttribute(SpellCustomAttributes.LaunchSourceIsExplicit))
                    handle.Entity.ApplyPositioning(casterRenderer.TagContainer, spellVisualEffect);
            }

            if (spellInfo.ExplicitTargetType == SpellExplicitTargetType.Destination)
            {
                float distance = Mathf.Clamp(Vector3.Distance(caster.Position, processingToken.Destination), StatUtils.DefaultCombatReach, float.MaxValue);
                float delay = spellInfo.Delay > 0 ? spellInfo.Delay / 1000.0f : distance / spellInfo.Speed;

                if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Projectile, out EffectSpellSettings destinationSettings))
                    spellVisualController.SpawnVisual(casterRenderer, processingToken.Source, processingToken.Destination, destinationSettings, delay, sourceIsExplicit);

                if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Destination, out EffectSpellSettings destinationEffect))
                    destinationEffect.EffectSettings.PlayEffect(processingToken.Destination + Vector3.up, caster.Rotation);
            }
                
        }
        
        private void OnSpellHit(Unit target, int spellId)
        {
            if (!unitRendererController.TryFind(target.Id, out UnitRenderer targetRenderer))
                return;

            if (!SpellVisuals.TryGetValue(spellId, out SpellVisualsInfo spellVisuals))
                return;

            if (spellVisuals.VisualsByUsage.TryGetValue(EffectSpellSettings.UsageType.Impact, out EffectSpellSettings spellVisualEffect))
            {
                EffectHandle handle = spellVisualEffect.EffectSettings.PlayEffect(target.Position, target.Rotation);
                if (handle.IsValid)
                    handle.Entity.ApplyPositioning(targetRenderer.TagContainer, spellVisualEffect);
            }
        }

        private void OnUnitModelAttached(UnitModel unitModel, UnitRenderer unitRenderer, bool isAttached)
        {
            IReadOnlyList<Collider> hitBoxes = unitModel.HitBoxes;
            for (int i = 0; i < hitBoxes.Count; i++)
            {
                if (isAttached)
                    unitRenderersByHitBoxes.Add(hitBoxes[i], unitRenderer);
                else
                    unitRenderersByHitBoxes.Remove(hitBoxes[i]);
            }

            IReadOnlyList<UnitProjectileHitBox> projectileHitBoxes = unitModel.ProjectileHitBoxes;
            for (int i = 0; i < projectileHitBoxes.Count; i++)
            {
                if (isAttached)
                {
                    unitRendererProjectileHitBoxesByCollider.Add(projectileHitBoxes[i].HitBoxCollider, unitRenderer);
                    unitProjectileHitBoxesByCollider.Add(projectileHitBoxes[i].HitBoxCollider, projectileHitBoxes[i]);
                }
                else
                {
                    unitRendererProjectileHitBoxesByCollider.Remove(projectileHitBoxes[i].HitBoxCollider);
                    unitProjectileHitBoxesByCollider.Remove(projectileHitBoxes[i].HitBoxCollider);
                }
            }
        }

        private void RegisterHandler(IUnitRendererHandler unitRendererHandler) => unitRendererController.RegisterHandler(unitRendererHandler);

        private void UnregisterHandler(IUnitRendererHandler unitRendererHandler) => unitRendererController.UnregisterHandler(unitRendererHandler);

        public AnimationInfo FindAnimation(SpellInfo spellInfo) =>
            animationInfoBySpellId.GetValueOrDefault(spellInfo.Id, spellAnimationInfoContainer.DefaultAnimation);

        public bool TryFindRendererByHitBox(Collider hitBox, out UnitRenderer unitRenderer) => unitRenderersByHitBoxes.TryGetValue(hitBox, out unitRenderer);

        public bool TryFindRendererByProjectileHitBox(Collider hitBox, out UnitRenderer unitRenderer, out UnitProjectileHitBox projectileHitBox)
        {
            unitProjectileHitBoxesByCollider.TryGetValue(hitBox, out projectileHitBox);
            return unitRendererProjectileHitBoxesByCollider.TryGetValue(hitBox, out unitRenderer);
        }
    }
}
