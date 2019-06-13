using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Balance Reference", menuName = "Game Data/Balance/Balance Reference", order = 2)]
    public class BalanceReference : ScriptableObject
    {
        [SerializeField, UsedImplicitly]
        private BalanceDefinition definition;

        private readonly List<MapDefinition> maps = new List<MapDefinition>();
        private readonly Dictionary<int, SpellInfo> spellInfosById = new Dictionary<int, SpellInfo>();

        public NetworkMovementType NetworkMovementType => definition.NetworkMovementType;
        public IReadOnlyList<MapDefinition> Maps => maps;
        public IReadOnlyDictionary<int, SpellInfo> SpellInfosById => spellInfosById;

        public void Initialize()
        {
            maps.AddRange(definition.MapEntries);
            definition.SpellInfos.ForEach(spellInfo => spellInfosById.Add(spellInfo.Id, spellInfo));

            foreach (var spellInfoEntry in SpellInfosById)
                spellInfoEntry.Value.Initialize();
        }

        public void Deinitialize()
        {
            foreach (var spellInfoEntry in SpellInfosById)
                spellInfoEntry.Value.Deinitialize();

            spellInfosById.Clear();
            maps.Clear();
        }
    }
}
