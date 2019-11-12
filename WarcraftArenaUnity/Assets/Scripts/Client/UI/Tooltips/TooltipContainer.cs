using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    public class TooltipContainer : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private TooltipItemNormal tooltipNormal;
        [SerializeField, UsedImplicitly] private TooltipItemSpell tooltipSpell;

        public static string ContainerTag => "Tooltip Container";

        public TooltipItemNormal TooltipNormal => tooltipNormal;
        public TooltipItemSpell TooltipSpell => tooltipSpell;
    }
}
