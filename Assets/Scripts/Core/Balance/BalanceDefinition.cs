using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Balance Definition", menuName = "Game Data/Balance/Balance Definition", order = 1)]
    public class BalanceDefinition : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private NetworkMovementType networkMovementType;
        [SerializeField, UsedImplicitly]
        private List<SpellInfo> spellInfos = new List<SpellInfo>();
        [SerializeField, UsedImplicitly]
        private List<MapDefinition> mapEntries;

        public List<SpellInfo> SpellInfos => spellInfos;
        public List<MapDefinition> MapEntries => mapEntries;
        public NetworkMovementType NetworkMovementType => networkMovementType;
    }
}
