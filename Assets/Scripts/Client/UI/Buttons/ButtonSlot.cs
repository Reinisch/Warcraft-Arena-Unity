using System.ComponentModel;
using Client.Sound;
using Common;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Client
{
    public class ButtonSlot : UIBehaviour, IPointerDownHandler, IDropHandler
    {
        [Inject] private EventBus eventBus;

        [SerializeField, UsedImplicitly] private HotkeyInputItem hotkeyInput;
        [SerializeField, UsedImplicitly] private RectTransform rectTransform;
        [SerializeField, UsedImplicitly] private ButtonContent buttonContent;
        [SerializeField, UsedImplicitly] private SoundEntry pressSound;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI hotkeyText;

        public RectTransform RectTransform => rectTransform;
        public ButtonContent ButtonContent => buttonContent;

        public void Initialize()
        {
            buttonContent.Initialize(this);

            eventBus.RegisterEvent<HotkeyState>(hotkeyInput, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
            eventBus.RegisterEvent(hotkeyInput, GameEvents.HotkeyBindingChanged, OnHotkeyBindingChanged);

            OnHotkeyBindingChanged();
        }

        public void Denitialize()
        {
            eventBus.UnregisterEvent<HotkeyState>(hotkeyInput, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
            eventBus.UnregisterEvent(hotkeyInput, GameEvents.HotkeyBindingChanged, OnHotkeyBindingChanged);

            buttonContent.Deinitialize();
        }

        public void DoUpdate()
        {
            buttonContent.DoUpdate();
        }

        [UsedImplicitly, Description("Also called from manually pressing button.")]
        public void Click()
        {
            if (!isActiveAndEnabled)
                return;

            if (!buttonContent.IsAlreadyPressed)
            {
                pressSound?.Play(transform.position);
                buttonContent.Activate();
            }
        }

        public void OnPointerDown(PointerEventData data)
        {
        }

        public void OnDrop(PointerEventData data)
        {
        }

        private void OnHotkeyStateChanged(HotkeyState state)
        {
            if(state == HotkeyState.Pressed)
                Click();

            buttonContent.HandleHotkeyState(state);
        }

        private void OnHotkeyBindingChanged()
        {
            hotkeyText.text = LocalizationReference.Localize(hotkeyInput);
        }
    }
}