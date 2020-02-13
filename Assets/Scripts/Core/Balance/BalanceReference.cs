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
        private readonly Dictionary<int, UnitInfoAI> unitInfoAIById = new Dictionary<int, UnitInfoAI>();
        private readonly Dictionary<ClassType, ClassInfo> classesByType = new Dictionary<ClassType, ClassInfo>();

        internal IReadOnlyDictionary<int, CreatureInfo> CreatureInfoById => definition.CreatureInfoById;
        internal IReadOnlyDictionary<int, VehicleInfo> VehicleInfoById => definition.VehicleInfoById;

        public FactionDefinition DefaultFaction => definition.DefaultFaction;
        public UnitMovementDefinition UnitMovementDefinition => definition.UnitMovementDefinition;
        public IReadOnlyList<MapDefinition> Maps => maps;
        public IReadOnlyDictionary<int, SpellInfo> SpellInfosById => spellInfosById;
        public IReadOnlyDictionary<int, AuraInfo> AuraInfosById => auraInfosById;
        public IReadOnlyDictionary<int, FactionDefinition> FactionsById => factionsById;
        public IReadOnlyDictionary<int, UnitInfoAI> UnitInfoAIById => unitInfoAIById;
        public IReadOnlyDictionary<ClassType, ClassInfo> ClassesByType => classesByType;
        public SpellInfoContainer Spells => definition.Spells;

        protected override void OnRegistered()
        {
            definition.Register();

            maps.AddRange(definition.MapEntries);

            for(int i = 0; i < definition.SpellInfos.Count; i++)
                spellInfosById.Add(definition.SpellInfos[i].Id, definition.SpellInfos[i]);

            for (int i = 0; i < definition.AuraInfos.Count; i++)
                auraInfosById.Add(definition.AuraInfos[i].Id, definition.AuraInfos[i]);

            for (int i = 0; i < definition.FactionEntries.Count; i++)
                factionsById.Add(definition.FactionEntries[i].FactionId, definition.FactionEntries[i]);

            for (int i = 0; i < definition.ClassInfos.Count; i++)
                classesByType.Add(definition.ClassInfos[i].ClassType, definition.ClassInfos[i]);

            for (int i = 0; i < definition.UnitAIEntries.Count; i++)
                unitInfoAIById.Add(definition.UnitAIEntries[i].Id, definition.UnitAIEntries[i]);
        }

        protected override void OnUnregister()
        {
            spellInfosById.Clear();
            auraInfosById.Clear();
            factionsById.Clear();
            unitInfoAIById.Clear();
            classesByType.Clear();
            maps.Clear();

            definition.Unregister();
        }

        public bool IsStealthAura(int auraId) => definition.IsStealthAura(auraId);
    }
}
