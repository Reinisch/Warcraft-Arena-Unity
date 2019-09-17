using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Balance Definition", menuName = "Game Data/Balance/Balance Definition", order = 1)]
    public class BalanceDefinition : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private FactionDefinition defaultFaction;
        [SerializeField, UsedImplicitly]
        private UnitMovementDefinition unitMovementDefinition;
        [SerializeField, UsedImplicitly]
        private List<SpellInfo> spellInfos = new List<SpellInfo>();
        [SerializeField, UsedImplicitly]
        private List<AuraInfo> auraInfo = new List<AuraInfo>();
        [SerializeField, UsedImplicitly]
        private List<MapDefinition> mapEntries;
        [SerializeField, UsedImplicitly]
        private List<FactionDefinition> factionEntries;

        public List<SpellInfo> SpellInfos => spellInfos;
        public List<AuraInfo> AuraInfos => auraInfo;
        public List<MapDefinition> MapEntries => mapEntries;
        public List<FactionDefinition> FactionEntries => factionEntries;
        public FactionDefinition DefaultFaction => defaultFaction;
        public UnitMovementDefinition UnitMovementDefinition => unitMovementDefinition;
    }
}
