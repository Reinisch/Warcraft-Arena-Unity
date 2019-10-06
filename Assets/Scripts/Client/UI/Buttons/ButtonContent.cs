using System;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Client
{
    public class ButtonContent : UIBehaviour, IPointerEnterHandler, IPointerExitHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField, UsedImplicitly] private TooltipReference tooltips;
        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private InputReference input;
        [SerializeField, UsedImplicitly] private RectTransform rectTransform;
        [SerializeField, UsedImplicitly] private Image contentImage;
        [SerializeField, UsedImplicitly] private Image cooldownImage;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI cooldownText;
        [SerializeField, UsedImplicitly] private Button button;
        [SerializeField, UsedImplicitly] private ButtonContentType contentType;
        [SerializeField, UsedImplicitly] private int itemId;

        public ButtonSlot ButtonSlot { get; private set; }
        public ButtonContentType ContentType => contentType;
        public Image ContentImage => contentImage;

        private PointerEventData manualPointerData;
        private SpellInfo spellInfo;

        private bool isPointerDown;
        private bool isHotkeyDown;

        private readonly char[] timerText = { ' ', ' ', ' ' };
        private bool showingTimer;

        public bool IsAlreadyPressed => isPointerDown || isHotkeyDown;

        public void Initialize(ButtonSlot buttonSlot)
        {
            manualPointerData = new PointerEventData(EventSystem.current);
            ButtonSlot = buttonSlot;

            UpdateContent();
        }

        public void Deinitialize()
        {
            ButtonSlot = null;
            isHotkeyDown = false;
            isPointerDown = false;
        }

        public void DoUpdate()
        {
            switch (contentType)
            {
                case ButtonContentType.Spell:
                    UpdateSpell();
                    break;
                case ButtonContentType.Empty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), $"Unknown button content: {contentType} with id: {itemId}");
            }
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

            spellInfo = null;
            ContentImage.sprite = null;
            ContentImage.enabled = false;
            cooldownText.text = string.Empty;
            cooldownImage.fillAmount = 0;
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

        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltips.Show(spellInfo, rectTransform, TooltipAlignment.FromTop, TooltipSize.Normal);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            tooltips.Hide();
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

        private void UpdateContent()
        {
            switch (contentType)
            {
                case ButtonContentType.Spell when balance.SpellInfosById.ContainsKey(itemId):
                    spellInfo = balance.SpellInfosById[itemId];
                    ContentImage.sprite = rendering.SpellVisualSettingsById.ContainsKey(itemId)
                        ? rendering.SpellVisualSettingsById[itemId].SpellIcon
                        : rendering.DefaultSpellIcon;
                    break;
                case ButtonContentType.Spell:
                case ButtonContentType.Empty:
                    Remove();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentType), $"Unknown button content: {contentType} with id: {itemId}");
            }
        }

        private void UpdateSpell()
        {
            Player player = input.Player;
            if (player == null)
                return;

            int cooldownTimeLeft;
            int cooldownTime;
            bool showTimer = showingTimer;

            if (!player.SpellHistory.HasGlobalCooldown || spellInfo.HasAttribute(SpellExtraAttributes.IgnoreGcd))
                cooldownTimeLeft = cooldownTime = 0;
            else
            {
                cooldownTimeLeft = player.SpellHistory.GlobalCooldownLeft;
                cooldownTime = player.SpellHistory.GlobalCooldown;
            }

            if (player.SpellHistory.HasCooldown(spellInfo.Id, out SpellCooldown spellCooldown) && spellCooldown.CooldownLeft > cooldownTimeLeft)
            {
                showTimer = true;
                cooldownTimeLeft = spellCooldown.CooldownLeft;
                cooldownTime = spellCooldown.Cooldown;
            }

            if (cooldownTimeLeft == 0)
            {
                cooldownText.SetCharArray(timerText, 0, 0);
                cooldownImage.fillAmount = 0;
                showingTimer = false;
            }
            else
            {
                if (showTimer)
                    cooldownText.SetCharArray(timerText.SetSpellTimerNonAlloc(cooldownTimeLeft, out int length), 0, length);
                else
                    cooldownText.SetCharArray(timerText, 0, 0);

                cooldownImage.fillAmount = (float) cooldownTimeLeft / cooldownTime;
                showingTimer = showTimer;
            }
        }
    }
}