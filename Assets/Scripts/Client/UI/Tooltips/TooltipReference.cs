using UnityEngine;
using Core;
using JetBrains.Annotations;

namespace Client
{
    public class TooltipReference : ScriptableReferenceClient
    {
        private static readonly Vector3[] Corners = new Vector3[4];

        [SerializeField, UsedImplicitly] private TooltipSettingsBySizeDictionary tooltipSizeSettings;
        [SerializeField, UsedImplicitly] private TooltipSettingsByAlignmentDictionary tooltipAlignmentSettings;
        [SerializeField, UsedImplicitly] private TooltipItemNormal tooltipNormal;
        [SerializeField, UsedImplicitly] private TooltipItemSpell tooltipSpell;

        private TooltipSlot currentTooltip;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            tooltipSizeSettings.Register();
            tooltipAlignmentSettings.Register();
        }

        protected override void OnUnregister()
        {
            Hide();

            tooltipSizeSettings.Unregister();
            tooltipAlignmentSettings.Unregister();

            base.OnUnregister();
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            if (currentTooltip != null)
                if (currentTooltip.TargetRect == null || !currentTooltip.TargetRect.gameObject.activeSelf)
                    Hide();
        }

        public void Show(SpellInfo spellInfo, RectTransform targetRect, TooltipAlignment alignment, TooltipSize size)
        {
            if (currentTooltip != null && currentTooltip.Item is TooltipItemSpell == false)
                currentTooltip.Hide();

            if (tooltipSpell.ModifyContent(spellInfo))
                Show(tooltipSpell.Slot, targetRect, alignment, size);
        }

        public void Show(string tooltipText, RectTransform targetRect, TooltipAlignment alignment, TooltipSize size)
        {
            if (currentTooltip != null && currentTooltip.Item is TooltipItemNormal == false)
                currentTooltip.Hide();

            if (tooltipNormal.ModifyContent(tooltipText))
                Show(tooltipNormal.Slot, targetRect, alignment, size);
        }

        public void Hide()
        {
            currentTooltip?.Hide();
            currentTooltip = null;
        }

        private void Show(TooltipSlot newTooltip, RectTransform targetRect, TooltipAlignment alignment, TooltipSize size)
        {
            currentTooltip = newTooltip;
            targetRect.GetWorldCorners(Corners);

            tooltipSizeSettings.Value(size).Modify(newTooltip);
            tooltipAlignmentSettings.Value(alignment).Modify(newTooltip, Corners);

            tooltipSpell.Slot.Show(targetRect);
        }
    }
}