using System.Globalization;
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
        [SerializeField, UsedImplicitly] private LocalizationReference localization;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellDescription;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellName;
        [SerializeField, UsedImplicitly] private Image spellIcon;

        private readonly object[] descriptionArguments = new object[SpellTooltipInfo.MaxArguments];
        private readonly object[] unknownArguments = { 'X', 'Y', 'Z', 'N', 'K' };

        private readonly NumberFormatInfo tooltipNumberFormat = new NumberFormatInfo {PercentPositivePattern = 1, PercentNegativePattern = 1};

        public override bool ModifyContent(SpellInfo spellInfo)
        {
            spellIcon.sprite = rendering.SpellVisualSettingsById.ContainsKey(spellInfo.Id)
                ? rendering.SpellVisualSettingsById[spellInfo.Id].SpellIcon
                : rendering.DefaultSpellIcon;

            if (localization.TooltipInfoBySpell.TryGetValue(spellInfo, out SpellTooltipInfo tooltipInfo))
            {
                spellName.text = tooltipInfo.SpellNameString.Value;

                for (int i = 0; i < tooltipInfo.ArgumentSettings.Count; i++)
                    descriptionArguments[i] = tooltipInfo.ArgumentSettings[i].Resolve() ?? unknownArguments[i];

                spellDescription.text = string.Format(tooltipNumberFormat, tooltipInfo.SpellDescriptionString.Value, descriptionArguments);

                return true;
            }

            Debug.LogError($"Missing tooltip for spell: {spellInfo.name}!");
            return false;
        }
    }
}