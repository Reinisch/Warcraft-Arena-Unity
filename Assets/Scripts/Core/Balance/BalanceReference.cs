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
        private readonly Dictionary<int, AuraInfo> auraInfosById = new Dictionary<int, AuraInfo>();
        private readonly Dictionary<int, FactionDefinition> factionsById = new Dictionary<int, FactionDefinition>();

        public FactionDefinition DefaultFaction => definition.DefaultFaction;
        public UnitMovementDefinition UnitMovementDefinition => definition.UnitMovementDefinition;
        public IReadOnlyList<MapDefinition> Maps => maps;
        public IReadOnlyDictionary<int, SpellInfo> SpellInfosById => spellInfosById;
        public IReadOnlyDictionary<int, AuraInfo> AuraInfosById => auraInfosById;
        public IReadOnlyDictionary<int, FactionDefinition> FactionsById => factionsById;

        protected override void OnRegistered()
        {
            maps.AddRange(definition.MapEntries);
            definition.SpellInfos.ForEach(spellInfo => spellInfosById.Add(spellInfo.Id, spellInfo));
            definition.AuraInfos.ForEach(auraInfo => auraInfosById.Add(auraInfo.Id, auraInfo));
            definition.FactionEntries.ForEach(factionEntry => factionsById.Add(factionEntry.FactionId, factionEntry));

            definition.SpellInfos.ForEach(spellInfo => spellInfo.PopulateEffectInfo());
        }

        protected override void OnUnregister()
        {
            spellInfosById.Clear();
            auraInfosById.Clear();
            factionsById.Clear();
            maps.Clear();
        }
    }
}
