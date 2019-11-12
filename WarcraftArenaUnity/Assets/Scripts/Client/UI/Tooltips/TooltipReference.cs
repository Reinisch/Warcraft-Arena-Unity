using UnityEngine;
using Core;
using JetBrains.Annotations;

namespace Client
{
    [CreateAssetMenu(fileName = "Tooltip Reference", menuName = "Game Data/Scriptable/Tooltips", order = 10)]
    public class TooltipReference : ScriptableReferenceClient
    {
        [SerializeField, UsedImplicitly] TooltipSettingsBySizeDictionary tooltipSizeSettings;
        [SerializeField, UsedImplicitly] TooltipSettingsByAlignmentDictionary tooltipAlignmentSettings;

        private TooltipContainer container;
        private TooltipSlot currentTooltip;

        private static readonly Vector3[] Corners = new Vector3[4];

        private TooltipItemNormal TooltipNormal => container.TooltipNormal;
        private TooltipItemSpell TooltipSpell => container.TooltipSpell;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            container = GameObject.FindGameObjectWithTag(TooltipContainer.ContainerTag).GetComponent<TooltipContainer>();
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

        public void Show(SpellInfo tooltipSpell, RectTransform targetRect, TooltipAlignment alignment, TooltipSize size)
        {
            if (currentTooltip != null && currentTooltip.Item is TooltipItemSpell == false)
                currentTooltip.Hide();

            if (TooltipSpell.ModifyContent(tooltipSpell))
                Show(TooltipSpell.Slot, targetRect, alignment, size);
        }

        public void Show(string tooltipText, RectTransform targetRect, TooltipAlignment alignment, TooltipSize size)
        {
            if (currentTooltip != null && currentTooltip.Item is TooltipItemNormal == false)
                currentTooltip.Hide();

            if (TooltipNormal.ModifyContent(tooltipText))
                Show(TooltipNormal.Slot, targetRect, alignment, size);
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

            TooltipSpell.Slot.Show(targetRect);
        }
    }
}