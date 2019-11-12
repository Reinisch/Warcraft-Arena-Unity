using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public abstract class TooltipItem<TContent> : TooltipItem
    {
        public abstract bool ModifyContent(TContent newContent);
    }

    public abstract class TooltipItem : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TooltipSlot slot;

        public TooltipSlot Slot => slot;
    }
}