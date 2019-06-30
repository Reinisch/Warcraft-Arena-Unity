using System.Collections.Generic;
using Common;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Balance Reference", menuName = "Game Data/Scriptable/Balance", order = 2)]
    public class BalanceReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly]
        private BalanceDefinition definition;

        private readonly List<MapDefinition> maps = new List<MapDefinition>();
        private readonly Dictionary<int, SpellInfo> spellInfosById = new Dictionary<int, SpellInfo>();
        private readonly Dictionary<int, FactionDefinition> factionsById = new Dictionary<int, FactionDefinition>();

        public FactionDefinition DefaultFaction => definition.DefaultFaction;
        public IReadOnlyList<MapDefinition> Maps => maps;
        public IReadOnlyDictionary<int, SpellInfo> SpellInfosById => spellInfosById;
        public IReadOnlyDictionary<int, FactionDefinition> FactionsById => factionsById;

        protected override void OnRegistered()
        {
            maps.AddRange(definition.MapEntries);
            definition.SpellInfos.ForEach(spellInfo => spellInfosById.Add(spellInfo.Id, spellInfo));
            definition.FactionEntries.ForEach(factionEntry => factionsById.Add(factionEntry.FactionId, factionEntry));
        }

        protected override void OnUnregister()
        {
            spellInfosById.Clear();
            factionsById.Clear();
            maps.Clear();
        }
    }
}
