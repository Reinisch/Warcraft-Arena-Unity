using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Common;

namespace Core
{
    public class BalanceManager : SingletonGameObject<BalanceManager>
    {
        [SerializeField, UsedImplicitly] private BalanceDefinition balanceDefinition;

        private readonly List<MapDefinition> maps = new List<MapDefinition>();
        private readonly List<SpellInfo> spellInfos = new List<SpellInfo>();
        private readonly Dictionary<int, MapDefinition> mapsById = new Dictionary<int, MapDefinition>();
        private readonly Dictionary<int, SpellInfo> spellInfosById = new Dictionary<int, SpellInfo>();

        public static NetworkMovementType NetworkMovementType => Instance.balanceDefinition.NetworkMovementType;
        public static IReadOnlyList<MapDefinition> Maps => Instance.maps;
        public static IReadOnlyList<SpellInfo> SpellInfos => Instance.spellInfos;
        public static IReadOnlyDictionary<int, MapDefinition> MapsById => Instance.mapsById;
        public static IReadOnlyDictionary<int, SpellInfo> SpellInfosById => Instance.spellInfosById;

        public new void Initialize()
        {
            base.Initialize();

            maps.AddRange(balanceDefinition.MapEntries);
            spellInfos.AddRange(balanceDefinition.SpellInfos);

            balanceDefinition.SpellInfos.ForEach(spellInfo => spellInfosById.Add(spellInfo.Id, spellInfo));
            balanceDefinition.MapEntries.ForEach(mapEntry => mapsById.Add(mapEntry.Id, mapEntry));

            foreach (var spellInfoEntry in SpellInfosById)
                spellInfoEntry.Value.Initialize();
        }

        public new void Deinitialize()
        {
            foreach (var spellInfoEntry in SpellInfosById)
                spellInfoEntry.Value.Deinitialize();

            spellInfosById.Clear();
            mapsById.Clear();

            maps.Clear();
            spellInfos.Clear();

            base.Deinitialize();
        }
    }
}