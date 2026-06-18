using Common;
using Core.Conditions;
using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core
{
    public class BalanceReference : ScriptableReference
    {
        [SerializeField, UsedImplicitly]
        private BalanceDefinition definition;

        [SerializeField, UsedImplicitly]
        private ConditionContainer conditionContainer;

        private readonly List<MapDefinition> maps = new();
        private readonly List<ScenarioDefinition> scenarios = new();
        private readonly Dictionary<int, SpellInfo> spellInfosById = new();
        private readonly Dictionary<int, FactionDefinition> factionsById = new();
        private readonly Dictionary<int, UnitInfoAI> unitInfoAIById = new();
        private readonly Dictionary<ClassType, ClassInfo> classesByType = new();
        private readonly Dictionary<int, CreatureInfo> creatureInfoById = new();
        private readonly Dictionary<int, VehicleInfo> vehicleInfoById = new();

        internal IReadOnlyDictionary<int, CreatureInfo> CreatureInfoById => creatureInfoById;
        internal IReadOnlyDictionary<int, VehicleInfo> VehicleInfoById => vehicleInfoById;

        public FactionDefinition DefaultFaction => definition.DefaultFaction;
        public UnitMovementDefinition UnitMovementDefinition => definition.UnitMovementDefinition;
        public IReadOnlyList<MapDefinition> Maps => maps;
        public IReadOnlyList<ScenarioDefinition> Scenarios => scenarios;
        public IReadOnlyDictionary<int, SpellInfo> SpellInfosById => spellInfosById;
        public IReadOnlyDictionary<int, FactionDefinition> FactionsById => factionsById;
        public IReadOnlyDictionary<int, UnitInfoAI> UnitInfoAIById => unitInfoAIById;
        public IReadOnlyDictionary<ClassType, ClassInfo> ClassesByType => classesByType;
        public SpellInfoContainer Spells => definition.Spells;

        /// <summary>Returns <paramref name="requested"/> if it's a defined/playable class, otherwise falls back to
        /// <see cref="ClassType.Mage"/>. Used to sanitise a class chosen in the lobby / sent by a joining client
        /// so an unknown value can never reach the spellbook build (which indexes <see cref="ClassesByType"/>).</summary>
        public ClassType ResolvePlayableClass(ClassType requested) =>
            classesByType.ContainsKey(requested) ? requested : ClassType.Mage;

        protected override void OnRegistered()
        {
            definition.Register();
            conditionContainer.Register();

            maps.AddRange(definition.MapEntries);
            scenarios.AddRange(definition.ScenarioEntries);

            foreach (SpellInfo spellInfo in definition.SpellInfos)
                spellInfosById.Add(spellInfo.Id, spellInfo);

            foreach (FactionDefinition faction in definition.FactionEntries)
                factionsById.Add(faction.FactionId, faction);

            foreach (ClassInfo classInfo in definition.ClassInfos)
                classesByType.Add(classInfo.ClassType, classInfo);

            foreach (UnitInfoAI unitAiInfo in definition.UnitAIEntries)
                unitInfoAIById.Add(unitAiInfo.Id, unitAiInfo);

            foreach (CreatureInfo creatureInfo in definition.CreatureEntries)
                creatureInfoById.Add(creatureInfo.Id, creatureInfo);

            foreach (VehicleInfo vehicleInfo in definition.VehicleEntries)
                vehicleInfoById.Add(vehicleInfo.Id, vehicleInfo);
        }

        protected override void OnUnregister()
        {
            spellInfosById.Clear();
            factionsById.Clear();
            unitInfoAIById.Clear();
            classesByType.Clear();
            creatureInfoById.Clear();
            vehicleInfoById.Clear();
            maps.Clear();
            scenarios.Clear();

            conditionContainer.Unregister();
            definition.Unregister();
        }

        protected override void QueueForInject(DiContainer container)
        {
            base.QueueForInject(container);

            conditionContainer.QueueForInject(container);
        }
    }
}
