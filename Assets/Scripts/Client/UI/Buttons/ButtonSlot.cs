using System.ComponentModel;
using Common;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Client
{
    public class ButtonSlot : UIBehaviour, IPointerDownHandler, IDropHandler
    {
        [SerializeField, UsedImplicitly] private HotkeyInputItem hotkeyInput;
        [SerializeField, UsedImplicitly] private RectTransform rectTransform;
        [SerializeField, UsedImplicitly] private ButtonContent buttonContent;
        [SerializeField, UsedImplicitly] private SoundEntry pressSound;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI timerText;
        [SerializeField, UsedImplicitly] private Image cooldownShade;

        public RectTransform RectTransform => rectTransform;

        public void Initialize()
        {
            buttonContent.Initialize(this);

            EventHandler.RegisterEvent<HotkeyState>(hotkeyInput, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);
        }

        public void Denitialize()
        {
            EventHandler.UnregisterEvent<HotkeyState>(hotkeyInput, GameEvents.HotkeyStateChanged, OnHotkeyStateChanged);

            buttonContent.Deinitialize();
        }

        public void DoUpdate()
        {
            buttonContent.DoUpdate();
        }

        [UsedImplicitly, Description("Also called from manually pressing button.")]
        public void Click()
        {
            if (!buttonContent.IsAlreadyPressed)
            {
                pressSound?.Play();
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
    }
}