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
        [SerializeField, UsedImplicitly] private UnitSoundController soundController;
        [SerializeField, UsedImplicitly] private float targetSmoothTime = 0.05f;

        private Vector3 targetSmoothVelocity;

        private readonly AuraEffectController auraEffectController = new AuraEffectController();
        private UnitModel model;
        private bool canAnimate = true;

        public TagContainer TagContainer => model == null ? dummyTagContainer : model.TagContainer;
        public Unit Unit { get; private set; }

        public void Initialize(Unit unit)
        {
            Unit = unit;
            transform.position = Unit.Position;

            DontDestroyOnLoad(gameObject);
            ReplaceModel(Unit.Model);
            OnScaleChanged();

            Unit.BoltEntity.AddEventListener(this);
            Unit.AddCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
            Unit.AddCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);
            Unit.AddCallback(nameof(IUnitState.EmoteType), OnEmoteTypeChanged);
            Unit.AddCallback(nameof(IUnitState.EmoteFrame), OnEmoteFrameChanged);
            EventHandler.RegisterEvent(Unit, GameEvents.UnitModelChanged, OnModelChanged);
            EventHandler.RegisterEvent(Unit, GameEvents.UnitScaleChanged, OnScaleChanged);

            auraEffectController.HandleAttach(this);
        }

        public void Deinitialize()
        {
            auraEffectController.HandleDetach();

            Unit.BoltEntity.RemoveEventListener(this);
            Unit.RemoveCallback(nameof(IUnitState.DeathState), OnDeathStateChanged);
            Unit.RemoveCallback(nameof(IUnitState.SpellCast), OnSpellCastChanged);
            Unit.RemoveCallback(nameof(IUnitState.EmoteType), OnEmoteTypeChanged);
            Unit.RemoveCallback(nameof(IUnitState.EmoteFrame), OnEmoteFrameChanged);
            EventHandler.UnregisterEvent(Unit, GameEvents.UnitModelChanged, OnModelChanged);
            EventHandler.UnregisterEvent(Unit, GameEvents.UnitScaleChanged, OnScaleChanged);

            ReplaceModel();
            CancelInvoke();

            Unit = null;
        }

        public void DoUpdate(float deltaTime)
        {
            transform.rotation = Unit.Rotation;
            transform.position = Vector3.SmoothDamp(transform.position, Unit.Position, ref targetSmoothVelocity, targetSmoothTime);

            model?.DoUpdate(deltaTime);
        }

        public override void OnEvent(UnitSpellLaunchEvent launchEvent)
        {
            base.OnEvent(launchEvent);

            if (!Unit.IsController)
            {
                var token = launchEvent.ProcessingEntries as SpellProcessingToken;
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, Unit, launchEvent.SpellId, token);
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

        private void OnScaleChanged() => transform.localScale = new Vector3(Unit.Scale, Unit.Scale, Unit.Scale);

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
            soundController.HandleModelChange(model);
        }

        private void HandleEmoteUpdate()
        {
            EmoteType emoteType = Unit.EmoteType;
            if (emoteType.IsState() || emoteType == EmoteType.None)
                DoEmote();
            else if (emoteType.IsOneShot() && BoltNetwork.Frame - Unit.EmoteFrame <= UnitUtils.EmoteOneShotFrameThreshold)
                DoEmote();
        }

        private void DoEmote(float cancellationDelay = 0.2f)
        {
            CancelInvoke(nameof(ResetEmoteTrigger));

            soundController.HandleEmote(Unit.EmoteType);
            model?.Animator.SetTrigger("Emote Trigger");
            model?.Animator.SetInteger("Emote", (int)Unit.EmoteType);

            Invoke(nameof(ResetEmoteTrigger), cancellationDelay);
        }

        private void ResetEmoteTrigger()
        {
            model?.Animator.ResetTrigger("Emote Trigger");
        }

        private void OnDeathStateChanged()
        {
            model?.Animator.SetBool("IsDead", Unit.IsDead);

            if(Unit.IsDead)
                soundController.PlayOneShot(UnitSounds.Death);
        }

        private void OnSpellCastChanged()
        {
            model?.Animator.SetBool("Casting", Unit.SpellCast.State.Id != 0);
        }

        private void OnEmoteTypeChanged()
        {
            HandleEmoteUpdate();
        }

        private void OnEmoteFrameChanged()
        {
            HandleEmoteUpdate();
        }
    }
}