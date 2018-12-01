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

    [SerializeField, UsedImplicitly] private KeyCode inputKey;
    [SerializeField, UsedImplicitly] private KeyCode modifierA;
    [SerializeField, UsedImplicitly] private KeyCode modifierB;

    ButtonContent buttonContent;

    public RectTransform RectTransform { get; private set; }
    public Image CooldownShade { get; private set; }
    public Text TimerText { get; private set; }

    public bool IsButtonPressed()
    {
        if (modifierA != KeyCode.None && !Input.GetKey(modifierA))
            return false;

        if (modifierB != KeyCode.None && !Input.GetKey(modifierB))
            return false;

        return Input.GetKeyDown(inputKey) && !(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftAlt));
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

        if (buttonContent != null)
            buttonContent.UpdateButton();
    }

    public void Click()
    {
        if (!InterfaceManager.Instance.ButtonController.IsDragging && buttonContent != null)
            buttonContent.Activate();
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (InterfaceManager.Instance.ButtonController.IsDragging)
        {
            InterfaceManager.Instance.ButtonController.DropItem(buttonContent);
            buttonContent.Enable();
        }
    }

    public void OnDrop(PointerEventData data)
    {
        if (InterfaceManager.Instance.ButtonController.IsDragging)
        {
            InterfaceManager.Instance.ButtonController.DropItem(buttonContent);
            buttonContent.Enable();
        }
    }
}
