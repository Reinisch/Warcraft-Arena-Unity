using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BalanceManager : SingletonGameObject<BalanceManager>
    {
        public static NetworkMovementType NetworkMovementType => Instance.balanceDefinition.NetworkMovementType;
        public static List<MapDefinition> Maps { get; } = new List<MapDefinition>();
        public static List<SpellInfo> SpellInfos { get; } = new List<SpellInfo>();
        public static Dictionary<int, MapDefinition> MapsById { get; } = new Dictionary<int, MapDefinition>();
        public static Dictionary<int, SpellInfo> SpellInfosById { get; } = new Dictionary<int, SpellInfo>();

        [SerializeField] private BalanceDefinition balanceDefinition;

        public void Initialize()
        {
            Maps.AddRange(balanceDefinition.MapEntries);
            SpellInfos.AddRange(balanceDefinition.SpellInfos);

            balanceDefinition.SpellInfos.ForEach(spellInfo => SpellInfosById.Add(spellInfo.Id, spellInfo));
            balanceDefinition.MapEntries.ForEach(mapEntry => MapsById.Add(mapEntry.Id, mapEntry));

            foreach (var spellInfoEntry in SpellInfosById)
                spellInfoEntry.Value.Initialize();
        }

        public void Deinitialize()
        {
            foreach (var spellInfoEntry in SpellInfosById)
                spellInfoEntry.Value.Deinitialize();

            SpellInfosById.Clear();
            MapsById.Clear();

            Maps.Clear();
            SpellInfos.Clear();
        }
    }
}