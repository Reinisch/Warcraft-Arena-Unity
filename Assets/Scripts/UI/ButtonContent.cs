using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler
{
    public int itemId;
    public ContentType ContentType;

    public ButtonSlot ButtonSlot { get; set; }
    public Image Image { get; set; }

    ActionBar actionBar;
    RectTransform rectTransform;
    Spell spell;

    bool hasCooldown = false;

    public void Initialize(ActionBar newActionBar, ButtonSlot buttonSlot)
    {
        Image = gameObject.GetComponent<Image>();
        actionBar = newActionBar;
        rectTransform = GetComponent<RectTransform>();

        ButtonSlot = buttonSlot;

        ArenaManager currentWorld = actionBar.playerInterface.world;
        if (currentWorld.SpellLibrary.HasSpell(itemId))
        {
            if (currentWorld.SpellIcons.ContainsKey(currentWorld.SpellLibrary.GetSpell(itemId).iconName))
            {
                Image.sprite = currentWorld.SpellIcons[currentWorld.SpellLibrary.GetSpell(itemId).iconName];
            }
            else
                Image.sprite = currentWorld.SpellIcons["Default"];
        }
        else
            Image.sprite = currentWorld.SpellIcons["Default"];

        if (currentWorld.PlayerUnit.Character.Spells.HasSpell(itemId))
            spell = currentWorld.PlayerUnit.Character.Spells.GetSpell(itemId);
        else
            spell = null;
    }

    public void UpdateButton()
    {
        if (spell != null)
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
        }
    }
    public void Enable()
    {
        if (actionBar.playerInterface.world.PlayerUnit.Character.Spells.HasSpell(itemId))
            spell = actionBar.playerInterface.world.PlayerUnit.Character.Spells.GetSpell(itemId);
        else
            spell = null;
        enabled = true;
    }
    public void Activate()
    {
        if (enabled)
        {
            actionBar.playerInterface.world.CastSpell(actionBar.playerInterface.PlayerUnit,
                actionBar.playerInterface.PlayerUnit.character.target, itemId);
        }
    }
    public void Remove()
    {
        itemId = 0;
        ContentType = ContentType.Empty;
        Image.sprite = null;
        Image.enabled = false;
        spell = null;
        enabled = false;
    }
    public void OnPointerEnter(PointerEventData data)
    {
        actionBar.playerInterface.ButtonController.ShowToolTip(rectTransform);
    }
    public void OnPointerDown(PointerEventData data)
    {
        if (actionBar.playerInterface.ButtonController.isDragging)
        {
            actionBar.playerInterface.ButtonController.DropItem(this);
            Enable();
        }
    }
    public void OnPointerExit(PointerEventData data)
    {
        actionBar.playerInterface.ButtonController.HideToolTip();
    }

    public void OnDrag(PointerEventData data)
    {
        if (hasCooldown)
        {
            Image.color = new Color(Image.color.r * 2, Image.color.g * 2, Image.color.b * 2, Image.color.a);
            ButtonSlot.CooldownShade.fillAmount = 0;
            ButtonSlot.TimerText.text = "";
            hasCooldown = false;
        }
        actionBar.playerInterface.ButtonController.DragItem(this);
        Remove();
    }
    public void FromDrag(ButtonContent draggedContent)
    {
        switch (draggedContent.ContentType)
        {
            case ContentType.Spell:
                itemId = draggedContent.itemId;
                ContentType = ContentType.Spell;
                Image.sprite = draggedContent.Image.sprite;
                break;
            default:
                break;
        }
    }
    public void FromDrop(ButtonContent droppedContent)
    {
        if (hasCooldown)
        {
            Image.color = new Color(Image.color.r * 2, Image.color.g * 2, Image.color.b * 2, Image.color.a);
            ButtonSlot.CooldownShade.fillAmount = 0;
            ButtonSlot.TimerText.text = "";
            hasCooldown = false;
        }

        switch (droppedContent.ContentType)
        {
            case ContentType.Spell:
                itemId = droppedContent.itemId;
                ContentType = ContentType.Spell;
                Image.sprite = droppedContent.Image.sprite;
                Image.enabled = true;
                if (actionBar.playerInterface.world.PlayerUnit.Character.Spells.HasSpell(itemId))
                    spell = actionBar.playerInterface.world.PlayerUnit.Character.Spells.GetSpell(itemId);
                else
                    spell = null;
                break;
            default:
                break;
        }
    }
    public void Replace(ButtonContent newContent)
    {
        if (hasCooldown)
        {
            Image.color = new Color(Image.color.r * 2, Image.color.g * 2, Image.color.b * 2, Image.color.a);
            ButtonSlot.CooldownShade.fillAmount = 0;
            ButtonSlot.TimerText.text = "";
            hasCooldown = false;
        }

        int newItemId = itemId;
        itemId = newContent.itemId;
        newContent.itemId = newItemId;

        ContentType newContentType = ContentType;
        ContentType = newContent.ContentType;
        newContent.ContentType = newContentType;

        Sprite newSprite = Image.sprite;
        Image.sprite = newContent.Image.sprite;
        newContent.Image.sprite = newSprite;

        Enable();
    }
}

public interface IButtonContainer
{
    void DragItem(ButtonContent buttonContent);
}