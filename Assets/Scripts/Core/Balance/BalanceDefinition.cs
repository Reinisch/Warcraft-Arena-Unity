using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Balance Definition", menuName = "Game Data/Balance Definiton", order = 1)]
    public class BalanceDefinition : ScriptableObject
    {
        [SerializeField]
        private List<SpellInfo> spellInfos = new List<SpellInfo>();
        [SerializeField]
        private List<MapDefinition> mapEntries;

        public List<SpellInfo> SpellInfos => spellInfos;
        public List<MapDefinition> MapEntries => mapEntries;
    }
}
