using Common;
using System.Collections.Generic;
using Client.Localization;
using Core;
using JetBrains.Annotations;
using UnityEngine;

namespace Client
{
    [CreateAssetMenu(fileName = "Localization Reference", menuName = "Game Data/Scriptable/Localization", order = 2)]
    public partial class LocalizationReference : Localization.LocalizationReference
    {
        [SerializeField, UsedImplicitly] private List<SpellCastResultLink> spellCastResults;

        private static readonly Dictionary<SpellCastResult, LocalizedString> StringsBySpellCastResult = new Dictionary<SpellCastResult, LocalizedString>();

        protected override void OnRegistered()
        {
            base.OnRegistered();

            spellCastResults.ForEach(item => StringsBySpellCastResult.Add(item.SpellCastResult, item.LocalizedString));
        }

        protected override void OnUnregister()
        {
            StringsBySpellCastResult.Clear();

            base.OnUnregister();
        }

        public static string Localize(SpellCastResult castResult)
        {
            Assert.IsTrue(StringsBySpellCastResult.ContainsKey(castResult), $"Missing localization for SpellCastResult: {castResult}");

            if (StringsBySpellCastResult.TryGetValue(castResult, out LocalizedString localizedString))
                return localizedString.Value;
            else
                return castResult.ToString();
        }
    }
}
