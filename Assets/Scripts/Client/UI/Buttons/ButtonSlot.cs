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
        [SerializeField, UsedImplicitly] private TextMeshProUGUI timerText;
        [SerializeField, UsedImplicitly] private Image cooldownShade;

        public RectTransform RectTransform => rectTransform;

        public void Initialize()
        {
            buttonContent.Initialize(this);

            EventHandler.RegisterEvent(hotkeyInput, GameEvents.HotkeyPressed, OnHotkeyPressed);
        }

        public void Denitialize()
        {
            EventHandler.UnregisterEvent(hotkeyInput, GameEvents.HotkeyPressed, OnHotkeyPressed);

            buttonContent.Deinitialize();
        }

        public void DoUpdate()
        {
            buttonContent.UpdateButton();
        }

        [UsedImplicitly]
        public void Click()
        {
            buttonContent.Activate();
        }

        public void OnPointerDown(PointerEventData data)
        {
            /*if (InterfaceManager.Instance.ButtonController.IsDragging)
            {
                InterfaceManager.Instance.ButtonController.DropItem(buttonContent);
                buttonContent.Enable();
            }*/
        }

        public void OnDrop(PointerEventData data)
        {
            /*if (InterfaceManager.Instance.ButtonController.IsDragging)
            {
                InterfaceManager.Instance.ButtonController.DropItem(buttonContent);
                buttonContent.Enable();
            }*/
        }

        private void OnHotkeyPressed()
        {
            Click();
        }
    }
}