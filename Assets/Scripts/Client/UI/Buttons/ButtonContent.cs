using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class ButtonContent : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private InputReference input;
        [SerializeField, UsedImplicitly] private Image contentImage;
        [SerializeField, UsedImplicitly] private Button button;
        [SerializeField, UsedImplicitly] private ButtonContentType contentType;
        [SerializeField, UsedImplicitly] private int itemId;

        public ButtonSlot ButtonSlot { get; private set; }
        public ButtonContentType ContentType => contentType;
        public Image ContentImage => contentImage;

        private PointerEventData manualPointerData;
        private bool isPointerDown;
        private bool isHotkeyDown;

        public bool IsAlreadyPressed => isPointerDown || isHotkeyDown;

        public void Initialize(ButtonSlot buttonSlot)
        {
            manualPointerData = new PointerEventData(EventSystem.current);
            ButtonSlot = buttonSlot;

            switch (contentType)
            {
                case ButtonContentType.Spell when balance.SpellInfosById.ContainsKey(itemId):
                    ContentImage.sprite = rendering.SpellVisualSettingsById.ContainsKey(itemId)
                        ? rendering.SpellVisualSettingsById[itemId].SpellIcon
                        : rendering.DefaultSpellIcon;
                    break;
                case ButtonContentType.Empty:
                    Remove();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), $"Unknown button content: {contentType} with id: {itemId}");
            }
        }

        public void Deinitialize()
        {
            ButtonSlot = null;
            isHotkeyDown = false;
            isPointerDown = false;
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
            if (!enabled)
                return;

            switch (contentType)
            {
                case ButtonContentType.Spell when balance.SpellInfosById.ContainsKey(itemId):
                    input.CastSpell(itemId);
                    break;
                case ButtonContentType.Empty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), $"Unknown button content type: {contentType}");
            }
        }

        public void Remove()
        {
            contentType = ButtonContentType.Empty;
            itemId = 0;

            ContentImage.sprite = null;
            ContentImage.enabled = false;
            enabled = false;
        }

        public void HandleHotkeyState(HotkeyState state)
        {
            isHotkeyDown = state == HotkeyState.Pressed;
            if (isHotkeyDown && !isPointerDown)
                button.OnPointerDown(manualPointerData);
            else if (!isHotkeyDown && !isPointerDown)
                button.OnPointerUp(manualPointerData);
        }

        public void OnPointerEnter(PointerEventData data)
        {
        }

        public void OnPointerDown(PointerEventData data)
        {
            isPointerDown = true;
        }

        public void OnPointerUp(PointerEventData data)
        {
            isPointerDown = false;
            if (isHotkeyDown)
                button.OnPointerDown(manualPointerData);
        }

        public void OnPointerExit(PointerEventData data)
        {
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
}