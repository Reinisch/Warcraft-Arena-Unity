using System.Globalization;
using Client.Localization;
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
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellRange;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellCastTime;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellCost;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI spellCooldown;
        [SerializeField, UsedImplicitly] private LocalizedString rangeFormatString;
        [SerializeField, UsedImplicitly] private LocalizedString cooldownFormatString;
        [SerializeField, UsedImplicitly] private LocalizedString castTimeFormatString;
        [SerializeField, UsedImplicitly] private LocalizedString castTimeInstantString;
        [SerializeField, UsedImplicitly] private Image spellIcon;

        private readonly object[] descriptionArguments = new object[SpellTooltipInfo.MaxArguments];
        private readonly object[] unknownArguments = { 'X', 'Y', 'Z', 'N', 'K' };

        private readonly NumberFormatInfo tooltipNumberFormat = new NumberFormatInfo {PercentPositivePattern = 1, PercentNegativePattern = 1};

        public override bool ModifyContent(SpellInfo spellInfo)
        {
            spellIcon.sprite = rendering.SpellVisuals.ContainsKey(spellInfo.Id)
                ? rendering.SpellVisuals[spellInfo.Id].SpellIcon
                : rendering.DefaultSpellIcon;

            if (localization.TooltipInfoBySpell.TryGetValue(spellInfo, out SpellTooltipInfo tooltipInfo))
            {
                spellName.text = tooltipInfo.SpellNameString.Value;

                // update spell description with configured arguments
                for (int i = 0; i < tooltipInfo.ArgumentSettings.Count; i++)
                    descriptionArguments[i] = tooltipInfo.ArgumentSettings[i].Resolve() ?? unknownArguments[i];

                for (int i = tooltipInfo.ArgumentSettings.Count; i < unknownArguments.Length; i++)
                    descriptionArguments[i] = unknownArguments[i];

                spellDescription.text = string.Format(tooltipNumberFormat, tooltipInfo.SpellDescriptionString.Value, descriptionArguments);

                // update spell range label
                float range = Mathf.Max(spellInfo.MaxRangeFriend, spellInfo.MaxRangeHostile);
                spellRange.text = Mathf.Approximately(range, 0.0f) ? string.Empty : string.Format(rangeFormatString.Value, range);

                // update cooldown label
                float cooldown = (float) spellInfo.CooldownTime / 1000;
                spellCooldown.text = cooldown <= 0 ? string.Empty : string.Format(cooldownFormatString.Value, cooldown);

                // update cast time label
                float castTime = (float)spellInfo.CastTime / 1000;
                spellCastTime.text = castTime <= 0 ? castTimeInstantString.Value : string.Format(castTimeFormatString.Value, castTime);

                // update cost
                spellCost.text = string.Empty;
                for (int i = 0; i < spellInfo.PowerCosts.Count; i++)
                {
                    SpellPowerCostInfo powerCost = spellInfo.PowerCosts[i];
                    if (powerCost.PowerCostPercentage > 0)
                    {
                        if (spellCost.text != string.Empty)
                            spellCost.text += " / ";

                        spellCost.text += string.Format(LocalizationReference.Localize(powerCost.SpellPowerType, true).Value, powerCost.PowerCostPercentage);
                    }

                    if (powerCost.PowerCost > 0)
                    {
                        if (spellCost.text != string.Empty)
                            spellCost.text += " / ";

                        spellCost.text += string.Format(LocalizationReference.Localize(powerCost.SpellPowerType, false).Value, powerCost.PowerCost);
                    }
                }

                if (spellInfo.HasAttribute(SpellAttributes.RequiresComboPoints))
                {
                    if (spellCost.text != string.Empty)
                        spellCost.text += " / ";

                    spellCost.text += LocalizationReference.Localize(SpellPowerType.ComboPoints, false).Value;
                }

                return true;
            }

            Debug.LogError($"Missing tooltip for spell: {spellInfo.name}!");
            return false;
        }
    }
}