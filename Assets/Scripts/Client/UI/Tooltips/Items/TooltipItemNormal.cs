using JetBrains.Annotations;
using TMPro;
using UnityEngine;

namespace Client
{
    public sealed class TooltipItemNormal : TooltipItem<string>
    {
        [SerializeField, UsedImplicitly] private TextMeshProUGUI tooltipText;

        public override bool ModifyContent(string text)
        {
            tooltipText.SetText(text);

            return true;
        }
    }
}