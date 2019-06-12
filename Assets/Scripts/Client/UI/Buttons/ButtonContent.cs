using UnityEngine.UI;
using UnityEngine.EventSystems;
using Client;
using Core;
using JetBrains.Annotations;
using UnityEngine;

public class ButtonContent : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler
{
    [SerializeField, UsedImplicitly] private BalanceReference balance;
    [SerializeField, UsedImplicitly] private RenderingReference rendering;
    [SerializeField, UsedImplicitly] private ButtonSlot.ContentType contentType;
    [SerializeField, UsedImplicitly] private int itemId;

    public ButtonSlot.ContentType ContentType => contentType;
    public ButtonSlot ButtonSlot { get; set; }
    public Image Image { get; set; }

    public void Initialize(ButtonSlot buttonSlot)
    {
        Image = gameObject.GetComponent<Image>();

        ButtonSlot = buttonSlot;
        Image.sprite = rendering.SpellVisualSettingsById.ContainsKey(itemId) 
            ? rendering.SpellVisualSettingsById[itemId].SpellIcon 
            : rendering.DefaultSpellIcon;
    }

    public void UpdateButton()
    {
        /*if (spell != null)
        {
            if (spell.spellCooldown.HasCooldown)
            {
                if (hasCooldown == false)
                {
                    Image.color = new Color(Image.color.r / 2, Image.color.g / 2, Image.color.b / 2, Image.color.a);
                    hasCooldown = true;
                }
                double timeLeft = spell.spellCooldown.timeLeft;
                if (timeLeft > 1)
                    timeLeft = Math.Round(timeLeft, 0);
                else
                    timeLeft = Math.Round(timeLeft, 1);

                if (timeLeft != 0)
                {
                    ButtonSlot.TimerText.text = timeLeft.ToString();
                    ButtonSlot.CooldownShade.fillAmount = spell.spellCooldown.timeLeft / spell.spellCooldown.duration;
                }
                else
                {
                    Image.color = new Color(Image.color.r * 2, Image.color.g * 2, Image.color.b * 2, Image.color.a);
                    ButtonSlot.CooldownShade.fillAmount = 0;
                    ButtonSlot.TimerText.text = "";
                    hasCooldown = false;
                }
            }
            else
            {
                if (hasCooldown)
                {
                    Image.color = new Color(Image.color.r * 2, Image.color.g * 2, Image.color.b * 2, Image.color.a);
                    ButtonSlot.CooldownShade.fillAmount = 0;
                    ButtonSlot.TimerText.text = "";
                    hasCooldown = false;
                }
            }
        }*/
    }

    public void Activate()
    {
        if (enabled && balance.SpellInfosById.ContainsKey(itemId))
            InputManager.CastSpell(itemId);
    }

    public void Remove()
    {
        itemId = 0;
        contentType = ButtonSlot.ContentType.Empty;
        Image.sprite = null;
        Image.enabled = false;
        enabled = false;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        //InterfaceManager.Instance.ButtonController.ShowToolTip(rectTransform);
    }

    public void OnPointerDown(PointerEventData data)
    {
        /*if (InterfaceManager.Instance.ButtonController.isDragging)
        {
            InterfaceManager.Instance.ButtonController.DropItem(this);
            Enable();
        }*/
    }

    public void OnPointerExit(PointerEventData data)
    {
        //InterfaceManager.Instance.ButtonController.HideToolTip();
    }

    public void OnDrag(PointerEventData data)
    {
    }

    public void FromDrag(ButtonContent draggedContent)
    {
    }

    public void FromDrop(ButtonContent droppedContent)
    {
    }

    public void Replace(ButtonContent newContent)
    {
    }
}