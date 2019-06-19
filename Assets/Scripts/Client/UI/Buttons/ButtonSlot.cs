using Client;
using Common;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonSlot : UIBehaviour, IPointerDownHandler, IDropHandler
{
    public enum ContentType
    {
        Spell, Empty
    }

    [SerializeField, UsedImplicitly] private HotkeyInputItem hotkeyInput;

    ButtonContent buttonContent;

    public RectTransform RectTransform { get; private set; }
    public Image CooldownShade { get; private set; }
    public Text TimerText { get; private set; }

    public void Initialize()
    {
        buttonContent = gameObject.GetComponentInChildren<ButtonContent>();
        RectTransform = GetComponent<RectTransform>();

        CooldownShade = transform.Find("Cooldown").GetComponent<Image>();
        TimerText = transform.Find("Timer").GetComponent<Text>();

        buttonContent?.Initialize(this);

        EventHandler.RegisterEvent(hotkeyInput, GameEvents.HotkeyPressed, OnHotkeyPressed);
    }

    public void Denitialize()
    {
        EventHandler.UnregisterEvent(hotkeyInput, GameEvents.HotkeyPressed, OnHotkeyPressed);
    }

    public void DoUpdate(float deltaTime)
    {
        buttonContent?.UpdateButton();
    }

    public void Click()
    {
        if (buttonContent != null)
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
