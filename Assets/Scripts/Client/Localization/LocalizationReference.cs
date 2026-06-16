using System;
using Common;
using System.Collections.Generic;
using Client.Localization;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

namespace Client
{
    public partial class LocalizationReference : Localization.LocalizationReference
    {
        [SerializeField, UsedImplicitly] private LocalizedString missingStringPlaceholder;
        [SerializeField, UsedImplicitly] private LocalizedString emptyStringPlaceholder;
        [SerializeField, UsedImplicitly] private SpellTooltipInfoContainer spellTooltipSettings;
        [SerializeField, UsedImplicitly] private List<HotKeyModifierLink> hotkeyModifiers;
        [SerializeField, UsedImplicitly] private List<KeyCodeLink> keyCodes;
        [SerializeField, UsedImplicitly] private List<SpellCastResultLink> spellCastResults;
        [SerializeField, UsedImplicitly] private List<SpellMissTypeLink> spellMissTypes;
        [SerializeField, UsedImplicitly] private List<PowerTypeCostLink> powerTypeCosts;

        private static readonly Dictionary<KeyCode, string> StringsByKeyCode = new();
        private static readonly Dictionary<HotkeyModifier, string> StringsByHotkeyModifier = new();
        private static readonly Dictionary<SpellCastResult, LocalizedString> StringsBySpellCastResult = new();
        private static readonly Dictionary<SpellMissType, LocalizedString> StringsBySpellMissType = new();
        private static readonly Dictionary<SpellPowerType, PowerTypeCostLink> StringsBySpellPowerType = new();

        private static LocalizedString MissingString;
        private static LocalizedString EmptyString;

        private readonly Dictionary<SpellInfo, SpellTooltipInfo> tooltipInfoBySpell = new();
        private readonly Dictionary<int, SpellTooltipInfo> tooltipInfoBySpellId = new();

        public IReadOnlyDictionary<SpellInfo, SpellTooltipInfo> TooltipInfoBySpell => tooltipInfoBySpell;
        public IReadOnlyDictionary<int, SpellTooltipInfo> TooltipInfoBySpellId => tooltipInfoBySpellId;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            MissingString = missingStringPlaceholder;
            EmptyString = emptyStringPlaceholder;

            spellTooltipSettings.Register();
            foreach (SpellTooltipInfo tooltipInfo in spellTooltipSettings.ItemList)
            {
                tooltipInfoBySpell.Add(tooltipInfo.SpellInfo, tooltipInfo);
                tooltipInfoBySpellId.Add(tooltipInfo.SpellInfo.Id, tooltipInfo);
            }

            keyCodes.ForEach(item => StringsByKeyCode.Add(item.KeyCode, item.String));
            hotkeyModifiers.ForEach(item => StringsByHotkeyModifier.Add(item.Modifier, item.String));
            spellCastResults.ForEach(item => StringsBySpellCastResult.Add(item.SpellCastResult, item.LocalizedString));
            spellMissTypes.ForEach(item => StringsBySpellMissType.Add(item.SpellMissType, item.LocalizedString));
            powerTypeCosts.ForEach(item => StringsBySpellPowerType.Add(item.PowerType, item));

            foreach (KeyCode item in Enum.GetValues(typeof(KeyCode)))
                if (!StringsByKeyCode.ContainsKey(item))
                    StringsByKeyCode[item] = item.ToString();

            foreach (HotkeyModifier item in Enum.GetValues(typeof(HotkeyModifier)))
                if (!StringsByHotkeyModifier.ContainsKey(item))
                    StringsByHotkeyModifier[item] = item.ToString();
        }

        protected override void OnUnregister()
        {
            StringsByKeyCode.Clear();
            StringsByHotkeyModifier.Clear();
            StringsBySpellCastResult.Clear();
            StringsBySpellPowerType.Clear();
            tooltipInfoBySpell.Clear();
            tooltipInfoBySpellId.Clear();
            spellTooltipSettings.Unregister();

            MissingString = null;
            EmptyString = null;

            base.OnUnregister();
        }

        protected override void QueueForInject(DiContainer container)
        {
            base.QueueForInject(container);

            spellTooltipSettings.QueueForInject(container);
        }

        public static LocalizedString Localize(SpellCastResult castResult)
        {
            Assert.IsTrue(StringsBySpellCastResult.ContainsKey(castResult), $"Missing localization for SpellCastResult: {castResult}");

            return StringsBySpellCastResult.GetValueOrDefault(castResult, MissingString);
        }

        public static LocalizedString Localize(SpellMissType spellMissType)
        {
            return StringsBySpellMissType.GetValueOrDefault(spellMissType, EmptyString);
        }

        public static LocalizedString Localize(SpellPowerType powerType, bool isPercentage)
        {
            Assert.IsTrue(StringsBySpellPowerType.ContainsKey(powerType), $"Missing localization for PowerType: {powerType}");

            if (StringsBySpellPowerType.TryGetValue(powerType, out PowerTypeCostLink powerTypeEntry))
                return isPercentage ? powerTypeEntry.LocalizedPercentageString : powerTypeEntry.LocalizedRawString;

            return MissingString;
        }

        public static string Localize(HotkeyInputItem hotkeyInput)
        {
            if (hotkeyInput.KeyCode == KeyCode.None)
                return string.Empty;

            string result = string.Empty;
            if (hotkeyInput.Modifier != HotkeyModifier.None)
                result = $"{StringsByHotkeyModifier[hotkeyInput.Modifier]}-";

            return $"{result}{StringsByKeyCode[hotkeyInput.KeyCode]}";
        }
    }
}
