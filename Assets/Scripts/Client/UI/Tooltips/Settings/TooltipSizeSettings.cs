using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TooltipSizeSettings
    {
        [SerializeField, UsedImplicitly] private int flexibleWidth;
        [SerializeField, UsedImplicitly] private int preferredWidth;

        public void Modify(TooltipSlot slot)
        {
            slot.LayoutElement.flexibleWidth = flexibleWidth;
            slot.LayoutElement.preferredWidth = preferredWidth;
        }
    }
}
