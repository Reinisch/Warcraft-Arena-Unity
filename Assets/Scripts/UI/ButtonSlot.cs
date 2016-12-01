using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public enum ContentType { Item, Spell, Empty }

public class ButtonSlot : MonoBehaviour, IPointerDownHandler, IDropHandler
{
    public KeyCode inputKey;
    public KeyCode modifier; 

    ButtonContent buttonContent;
    ActionBar actionBar;

    public RectTransform RectTransform { get; set; }
    public Image CooldownShade { get; set; }
    public Text TimerText { get; set; }

    public bool IsButtonPressed()
    {
        if (modifier != KeyCode.None)
            return Input.GetKeyDown(inputKey) && Input.GetKey(modifier);
        else
            return Input.GetKeyDown(inputKey) && !(Input.GetKey(KeyCode.LeftControl)
                || Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.LeftAlt));
    }

    public virtual void Initialize(ActionBar newActionBar)
    {
        actionBar = newActionBar;
        buttonContent = gameObject.GetComponentInChildren<ButtonContent>();
        RectTransform = GetComponent<RectTransform>();

        CooldownShade = transform.FindChild("Cooldown").GetComponent<Image>();
        TimerText = transform.FindChild("Timer").GetComponent<Text>();

        if (buttonContent != null)
            buttonContent.Initialize(actionBar, this);
    }

    public void UpdateSlot()
    {
        if (buttonContent != null)
            buttonContent.UpdateButton();
    }

    public void Click()
    {
        if (!actionBar.playerInterface.ButtonController.isDragging)
        {
            if (buttonContent != null)
                buttonContent.Activate();
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        if (actionBar.playerInterface.ButtonController.isDragging)
        {
            actionBar.playerInterface.ButtonController.DropItem(buttonContent);
            buttonContent.Enable();
        }
    }

    public void OnDrop(PointerEventData data)
    {
        if (actionBar.playerInterface.ButtonController.isDragging)
        {
            actionBar.playerInterface.ButtonController.DropItem(buttonContent);
            buttonContent.Enable();
        }
    }
}
