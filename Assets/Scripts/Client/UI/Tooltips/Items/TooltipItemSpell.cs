using Core;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public sealed class TooltipItemSpell : TooltipItem<SpellInfo>
    {
        [SerializeField, UsedImplicitly] private RenderingReference rendering;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellDescription;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellName;
        [SerializeField, UsedImplicitly] private Image spellIcon;

        public override void ModifyContent(SpellInfo spellInfo)
        {
            spellName.text = spellInfo.SpellName;
            spellIcon.sprite = rendering.SpellVisualSettingsById.ContainsKey(spellInfo.Id)
                ? rendering.SpellVisualSettingsById[spellInfo.Id].SpellIcon
                : rendering.DefaultSpellIcon;
        }
    }
}