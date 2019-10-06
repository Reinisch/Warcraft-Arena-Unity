using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public abstract class TooltipItem<TContent> : TooltipItem
    {
        public abstract void ModifyContent(TContent newContent);
    }

    public abstract class TooltipItem : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TooltipSlot slot;

        public TooltipSlot Slot => slot;
    }
}