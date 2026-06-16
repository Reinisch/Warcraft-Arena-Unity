using Client.Sound;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public sealed partial class UnitRenderer : MonoBehaviour
    {
        [Inject] private RenderingReference rendering;
        [Inject] private EventBus eventBus;
        [SerializeField, UsedImplicitly] private TagContainer dummyTagContainer;
        [SerializeField, UsedImplicitly] private UnitSoundController soundController;

        private Vector3 targetSmoothVelocity;

        private readonly AuraEffectController auraEffectController = new AuraEffectController();
        private UnitModel model;
        private bool canAnimate = true;

        public TagContainer TagContainer => model == null ? dummyTagContainer : model.TagContainer;
        public Unit Unit { get; private set; }
        public UnitModel Model => model;

        public void Attach(Unit unit)
        {
            Unit = unit;
            transform.position = Unit.Position;

            ReplaceModel(Unit.Model, UnitModelReplacementMode.ScopeIn);
            OnScaleChanged();

            Unit.EventTeleported += OnTeleportation;
            Unit.Attributes.EventDeathStateChanged += OnDeathStateChanged;
            Unit.Attributes.EventEmoteStateChanged += OnEmoteTypeChanged;
            Unit.SpellCast.EventSpellCastChanged += OnSpellCastChanged;
            eventBus.RegisterEvent(Unit, GameEvents.UnitModelChanged, OnModelChanged);
            eventBus.RegisterEvent(Unit, GameEvents.UnitScaleChanged, OnScaleChanged);
            eventBus.RegisterEvent(Unit, GameEvents.UnitVisualsChanged, OnVisualsChanged);

            auraEffectController.HandleAttach(this);
        }

        public UnitModel Detach(UnitModelReplacementMode mode)
        {
            auraEffectController.HandleDetach();

            Unit.Attributes.EventDeathStateChanged -= OnDeathStateChanged;
            Unit.Attributes.EventEmoteStateChanged -= OnEmoteTypeChanged;
            Unit.SpellCast.EventSpellCastChanged -= OnSpellCastChanged;
            Unit.EventTeleported -= OnTeleportation;
            eventBus.UnregisterEvent(Unit, GameEvents.UnitModelChanged, OnModelChanged);
            eventBus.UnregisterEvent(Unit, GameEvents.UnitScaleChanged, OnScaleChanged);
            eventBus.UnregisterEvent(Unit, GameEvents.UnitVisualsChanged, OnVisualsChanged);

            CancelInvoke();

            UnitModel lastModel = ReplaceModel(mode: mode);
            Unit = null;
            return lastModel;
        }

        public void DoUpdate(float deltaTime)
        {
            transform.rotation = Unit.Rotation;
            transform.position = Vector3.SmoothDamp(transform.position, Unit.Position,
                ref targetSmoothVelocity, rendering.UnitRendererSettings.RenderInterpolationSmoothTime);

            model?.DoUpdate(this, deltaTime);
        }

        public void TriggerInstantCast(SpellInfo spellInfo)
        {
            if (canAnimate)
                model?.TriggerInstantCast(spellInfo);
        }

        public void PlayOneShot(SoundEntry soundEntry)
        {
            soundController.PlayOneShot(soundEntry);
        }

        private void UpdateAnimationState(bool enabled)
        {
            canAnimate = enabled;
            if (model != null)
                model.Animator.speed = canAnimate ? 1.0f : 0.0f;
        }

        private void OnModelChanged() => ReplaceModel(Unit.Model, UnitModelReplacementMode.Transformation);

        private void OnScaleChanged() => transform.localScale = new Vector3(Unit.Scale, Unit.Scale, Unit.Scale);

        private void OnVisualsChanged() => HandleVisualEffects(false);

        private void ReplaceModel(int modelId, UnitModelReplacementMode mode)
        {
            Assert.IsTrue(mode != UnitModelReplacementMode.ScopeOut);

            if (model != null && model.Settings.Id == modelId)
                return;

            if (rendering.Models.TryGetValue(modelId, out UnitModelSettings newModelSettings))
            {
                UnitModel newModel = GameObjectPool.Take(newModelSettings.Prototype);
                var modelInitializer = new UnitModelInitializer
                {
                    UnitRenderer = this,
                    ModelSettings = newModelSettings,
                    PreviousModel = model,
                    ReplacementMode = mode
                };

                newModel.Initialize(modelInitializer);
                ReplaceModel(newModel);
            }
            else
                Debug.LogError($"Missing model with id: {modelId}");
        }

        private UnitModel ReplaceModel(UnitModel newModel = null, UnitModelReplacementMode mode = UnitModelReplacementMode.ScopeIn)
        {
            UnitModel oldModel = model;
            if (model != null && newModel != null)
                model.TagContainer.TransferChildren(newModel.TagContainer);

            model = newModel;
            UpdateAnimationState(canAnimate);
            soundController.HandleModelChange(model);

            if (oldModel != null)
                eventBus.ExecuteEvent(rendering, GameEvents.UnitModelAttached, oldModel, this, false);

            if (newModel != null)
                eventBus.ExecuteEvent(rendering, GameEvents.UnitModelAttached, newModel, this, true);

            if (oldModel != null && mode != UnitModelReplacementMode.ScopeOut)
            {
                oldModel.Deinitialize();
                oldModel = null;
            }

            return oldModel;
        }

        private void HandleVisualEffects(bool instantly) => model?.HandleVisualEffects(this, instantly);

        private void HandleEmoteUpdate()
        {
            EmoteType emoteType = Unit.EmoteType;
            if (emoteType.IsState() || emoteType == EmoteType.None)
                DoEmote();
            else if (emoteType.IsOneShot() && Time.time - Unit.EmoteFrame <= UnitUtils.EmoteOneShotFrameThreshold)
                DoEmote();
        }

        private void OnTeleportation()
        {
            transform.position = Unit.Position;
        }

        private void DoEmote()
        {
            soundController.HandleEmote(Unit.EmoteType);
            model?.Animator.SetTrigger("Emote Trigger");
            model?.Animator.SetInteger("Emote", (int)Unit.EmoteType);
        }

        private void OnDeathStateChanged()
        {
            model?.Animator.SetBool("IsDead", Unit.IsDead);

            if(Unit.IsDead)
                soundController.PlayOneShot(UnitSounds.Death);
        }

        private void OnSpellCastChanged()
        {
            model?.Animator.SetBool("Casting", Unit.SpellCast.IsCasting);
        }

        private void OnEmoteTypeChanged()
        {
            HandleEmoteUpdate();
        }
    }
}