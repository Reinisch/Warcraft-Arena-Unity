using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [Serializable]
    public class TooltipAlignmentSettings
    {
        [SerializeField, UsedImplicitly] private Vector2 pivot;
        [SerializeField, UsedImplicitly] private Vector2 anghoredPosition;
        [SerializeField, UsedImplicitly] private TextAnchor alignment;
        [SerializeField, UsedImplicitly] private uint[] worldCornerIndexes;

        public void Modify(TooltipSlot slot, Vector3[] corners)
        {
            slot.SelfRect.pivot = pivot;
            slot.SelfRect.anchoredPosition += anghoredPosition;
            slot.LayoutGroup.childAlignment = alignment;

            Vector3 worldPosition = Vector3.zero;
            int cornerCount = 0;
            for (int i = 0; i < worldCornerIndexes.Length; i++)
                if (worldCornerIndexes[i] < corners.Length)
                {
                    worldPosition += corners[worldCornerIndexes[i]];
                    cornerCount++;
                }

            if (cornerCount == 0)
                worldPosition = (corners[1] + corners[3]) / 2;
            else
                worldPosition /= cornerCount;

            slot.SelfRect.position = worldPosition;
        }
    }
}
