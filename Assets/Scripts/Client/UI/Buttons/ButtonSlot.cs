using Client;
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

    public bool IsButtonPressed()
    {
        return hotkeyInput.IsPressed();
    }

    public override void Initialize()
    {
        base.Initialize();

        buttonContent = gameObject.GetComponentInChildren<ButtonContent>();
        RectTransform = GetComponent<RectTransform>();

        CooldownShade = transform.Find("Cooldown").GetComponent<Image>();
        TimerText = transform.Find("Timer").GetComponent<Text>();

        if (buttonContent != null)
            buttonContent.Initialize(this);
    }

    public override void DoUpdate(int deltaTime)
    {
        base.DoUpdate(deltaTime);

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
}
