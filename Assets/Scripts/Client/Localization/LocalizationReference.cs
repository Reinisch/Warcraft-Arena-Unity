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
        [SerializeField, UsedImplicitly] private LocalizedString missingStringPlaceholder;
        [SerializeField, UsedImplicitly] private List<SpellCastResultLink> spellCastResults;
        [SerializeField, UsedImplicitly] private List<ClientConnectFailReasonLink> clientConnectFailReasons;

        private static readonly Dictionary<SpellCastResult, LocalizedString> StringsBySpellCastResult = new Dictionary<SpellCastResult, LocalizedString>();
        private static readonly Dictionary<ClientConnectFailReason, LocalizedString> StringsByClientConnectFailReason = new Dictionary<ClientConnectFailReason, LocalizedString>();
        private static LocalizedString MissingString;

        protected override void OnRegistered()
        {
            base.OnRegistered();

            MissingString = missingStringPlaceholder;
            spellCastResults.ForEach(item => StringsBySpellCastResult.Add(item.SpellCastResult, item.LocalizedString));
            clientConnectFailReasons.ForEach(item => StringsByClientConnectFailReason.Add(item.FailReason, item.LocalizedString));
        }

        protected override void OnUnregister()
        {
            StringsBySpellCastResult.Clear();
            StringsByClientConnectFailReason.Clear();
            MissingString = null;

            base.OnUnregister();
        }

        public static LocalizedString Localize(SpellCastResult castResult)
        {
            Assert.IsTrue(StringsBySpellCastResult.ContainsKey(castResult), $"Missing localization for SpellCastResult: {castResult}");

            if (StringsBySpellCastResult.TryGetValue(castResult, out LocalizedString localizedString))
                return localizedString;

            return MissingString;
        }

        public static LocalizedString Localize(ClientConnectFailReason failReason)
        {
            Assert.IsTrue(StringsByClientConnectFailReason.ContainsKey(failReason), $"Missing localization for ClientConnectFailReason: {failReason}");

            if (StringsByClientConnectFailReason.TryGetValue(failReason, out LocalizedString localizedString))
                return localizedString;

            return MissingString;
        }
    }
}
