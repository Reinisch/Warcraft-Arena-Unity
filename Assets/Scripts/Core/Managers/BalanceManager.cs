using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BalanceManager : SingletonGameObject<BalanceManager>
    {
        public static Dictionary<int, MapEntry> MapEntries { get; } = new Dictionary<int, MapEntry>();
        public static Dictionary<int, SpellInfo> SpellInfos { get; } = new Dictionary<int, SpellInfo>();

        [SerializeField] private BalanceDefinition balanceDefinition;

        public override void Initialize()
        {
            base.Initialize();

            balanceDefinition.SpellInfos.ForEach(spellInfo => SpellInfos.Add(spellInfo.Id, spellInfo));
            balanceDefinition.MapEntries.ForEach(mapEntry => MapEntries.Add(mapEntry.Id, mapEntry));

            foreach (var spellInfoEntry in SpellInfos)
                spellInfoEntry.Value.Initialize();
        }

        public override void Deinitialize()
        {
            foreach (var spellInfoEntry in SpellInfos)
                spellInfoEntry.Value.Deinitialize();

            SpellInfos.Clear();
            MapEntries.Clear();

            base.Deinitialize();
        }
    }
}