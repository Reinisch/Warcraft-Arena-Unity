using Bolt;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public sealed partial class UnitRenderer : EntityEventListener<IUnitState>
    {
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private TagContainer dummyTagContainer;

        private readonly AuraEffectController auraEffectController = new AuraEffectController();
        private UnitModel model;
        private bool canAnimate = true;

        public TagContainer TagContainer => model == null ? dummyTagContainer : model.TagContainer;
        public Unit Unit { get; private set; }

        public void Initialize(Unit unit)
        {
            Unit = unit;

            transform.SetParent(Unit.transform, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;

            ReplaceModel(Unit.Model);

            Unit.BoltEntity.AddEventListener(this);
            Unit.AddCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
            Unit.AddCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);
            EventHandler.RegisterEvent(Unit, GameEvents.UnitModelChanged, OnModelChanged);

            auraEffectController.HandleAttach(this);
        }

        public void Deinitialize()
        {
            auraEffectController.HandleDetach();

            Unit.BoltEntity.RemoveEventListener(this);
            Unit.RemoveCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
            Unit.RemoveCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);
            EventHandler.UnregisterEvent(Unit, GameEvents.UnitModelChanged, OnModelChanged);

            ReplaceModel();

            Unit = null;
        }

        public void DoUpdate(float deltaTime)
        {
            model?.DoUpdate(deltaTime);
        }

        public override void OnEvent(UnitSpellLaunchEvent launchEvent)
        {
            base.OnEvent(launchEvent);

            if (!Unit.IsController)
            {
                var token = launchEvent.ProcessingEntries as SpellProcessingToken;
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, Unit, launchEvent.SpellId, token, launchEvent.Source);
            }
        }

        public override void OnEvent(UnitSpellDamageEvent spellDamageEvent)
        {
            base.OnEvent(spellDamageEvent);

            if (canAnimate)
            {
                var hitType = (HitType)spellDamageEvent.HitType;
                model?.Animator.SetBool("WoundedCrit", hitType.HasTargetFlag(HitType.CriticalHit));
                model?.Animator.SetTrigger("Wound");
            }
        }

        public override void OnEvent(UnitSpellHitEvent spellHitEvent)
        {
            base.OnEvent(spellHitEvent);

            EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellHit, Unit, spellHitEvent.SpellId);
        }

        public void TriggerInstantCast()
        {
            if (canAnimate)
                model?.TriggerInstantCast();
        } 

        private void UpdateAnimationState(bool enabled)
        {
            canAnimate = enabled;
            if (model != null)
                model.Animator.speed = canAnimate ? 1.0f : 0.0f;
        }

        private void OnModelChanged() => ReplaceModel(Unit.Model);
      
        private void ReplaceModel(int modelId)
        {
            if (model != null && model.Settings.Id == modelId)
                return;

            if (rendering.ModelSettingsById.TryGetValue(modelId, out UnitModelSettings newModelSettings))
            {
                UnitModel newModel = GameObjectPool.Take(newModelSettings.Prototype);
                newModel.Initialize(this, newModelSettings);

                ReplaceModel(newModel);
            }
            else
                Debug.LogError($"Missing model with id: {modelId}");
        }

        private void ReplaceModel(UnitModel newModel = null)
        {
            if (model != null)
            {
                if (newModel != null)
                    model.TagContainer.TransferChildren(newModel.TagContainer);

                model.Deinitialize();
                GameObjectPool.Return(model, false);
            }

            model = newModel;
            UpdateAnimationState(canAnimate);
        }

        private void OnDeathStateChanged()
        {
            model?.Animator.SetBool("IsDead", Unit.IsDead);
        }

        private void OnSpellCastChanged()
        {
            model?.Animator.SetBool("Casting", Unit.SpellCast.State.Id != 0);
        }
    }
}