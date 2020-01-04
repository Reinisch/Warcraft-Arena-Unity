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
        [SerializeField, UsedImplicitly] private TextMeshProUGUI chargeText;
        [SerializeField, UsedImplicitly] private Button button;
        [SerializeField, UsedImplicitly] private TooltipAlignment tooltipAlignment = TooltipAlignment.FromTop;

        public ButtonSlot ButtonSlot { get; private set; }
        public ButtonContentType ContentType => data.ActionType;
        public Image ContentImage => contentImage;

        private readonly ActionButtonData data = new ActionButtonData(0, ButtonContentType.Empty);
        private PointerEventData manualPointerData;
        private SpellInfo spellInfo;

        private bool isPointerDown;
        private bool isHotkeyDown;

        private readonly char[] timerText = new char[3];
        private readonly char[] chargeCountText = new char[11];
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
            switch (data.ActionType)
            {
                case ButtonContentType.Spell:
                    UpdateSpell();
                    break;
                case ButtonContentType.Empty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.ActionType),
                        $"Unknown button content: {data.ActionType} with id: {data.ActionId}");
            }
        }

        public void UpdateContent(ActionButtonData newData)
        {
            data.Modify(newData);

            UpdateContent();
        }

        public void Activate()
        {
            if (!enabled)
                return;

            switch (data.ActionType)
            {
                case ButtonContentType.Spell when balance.SpellInfosById.ContainsKey(data.ActionId):
                    input.CastSpell(data.ActionId);
                    break;
                case ButtonContentType.Empty:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.ActionType),
                        $"Unknown button content type: {data.ActionType}");
            }
        }

        public void Remove()
        {
            data.Reset();

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
            tooltips.Show(spellInfo, rectTransform, tooltipAlignment, TooltipSize.Normal);
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
            switch (data.ActionType)
            {
                case ButtonContentType.Spell when balance.SpellInfosById.ContainsKey(data.ActionId):
                    spellInfo = balance.SpellInfosById[data.ActionId];
                    ContentImage.sprite = rendering.SpellVisuals.ContainsKey(data.ActionId)
                        ? rendering.SpellVisuals[data.ActionId].SpellIcon
                        : rendering.DefaultSpellIcon;

                    ContentImage.enabled = true;
                    enabled = true;
                    break;
                case ButtonContentType.Spell:
                case ButtonContentType.Empty:
                    Remove();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(data.ActionType), $"Unknown button content: {data.ActionType} with id: {data.ActionId}");
            }
        }

        private void UpdateSpell()
        {
            Player player = input.Player;
            if (player == null)
                return;

            int cooldownTimeLeft;
            int cooldownTime;
            int availableCharges = 0;
            bool showTimer = showingTimer;

            if (!player.SpellHistory.HasGlobalCooldown || spellInfo.HasAttribute(SpellExtraAttributes.IgnoreGcd))
                cooldownTimeLeft = cooldownTime = 0;
            else
            {
                cooldownTimeLeft = player.SpellHistory.GlobalCooldownLeft;
                cooldownTime = player.SpellHistory.GlobalCooldown;
            }

            if (spellInfo.IsUsingCharges)
            {
                bool hasCharge = player.SpellHistory.HasCharge(spellInfo, out SpellChargeCooldown chargeCooldown, out availableCharges);
                if (!hasCharge && chargeCooldown.ChargeTimeLeft > cooldownTimeLeft)
                {
                    showTimer = true;
                    cooldownTimeLeft = chargeCooldown.ChargeTimeLeft;
                    cooldownTime = chargeCooldown.ChargeTime;
                }
                else if (hasCharge && chargeCooldown != null)
                {
                    showTimer = false;
                    cooldownTimeLeft = chargeCooldown.ChargeTimeLeft;
                    cooldownTime = chargeCooldown.ChargeTime;
                }
            }
            else if (player.SpellHistory.HasCooldown(spellInfo.Id, out SpellCooldown spellCooldown) && spellCooldown.CooldownLeft > cooldownTimeLeft)
            {
                showTimer = true;
                cooldownTimeLeft = spellCooldown.CooldownLeft;
                cooldownTime = spellCooldown.Cooldown;
            }

            if (spellInfo.IsUsingCharges)
                chargeText.SetCharArray(chargeCountText.SetIntNonAlloc(availableCharges, out int length), 0, length);
            else
                chargeText.SetCharArray(chargeCountText, 0, 0);

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