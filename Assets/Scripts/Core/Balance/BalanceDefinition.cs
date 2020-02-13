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
        private SpellInfoContainer spellContainer;
        [SerializeField, UsedImplicitly]
        private AuraInfoContainer auraContainer;
        [SerializeField, UsedImplicitly]
        private ClassInfoContainer classContainer;
        [SerializeField, UsedImplicitly]
        private UnitInfoAIContainer unitAIContainer;
        [SerializeField, UsedImplicitly]
        private CreatureInfoContainer creatureContainer;
        [SerializeField, UsedImplicitly]
        private VehicleInfoContainer vehicleContainer;
        [SerializeField, UsedImplicitly]
        private List<MapDefinition> mapEntries;
        [SerializeField, UsedImplicitly]
        private List<FactionDefinition> factionEntries;

        internal IReadOnlyDictionary<int, CreatureInfo> CreatureInfoById => creatureContainer.CreatureInfoById;
        internal IReadOnlyDictionary<int, VehicleInfo> VehicleInfoById => vehicleContainer.VehicleInfoById;

        public IReadOnlyList<SpellInfo> SpellInfos => spellContainer.ItemList;
        public IReadOnlyList<AuraInfo> AuraInfos => auraContainer.ItemList;
        public IReadOnlyList<ClassInfo> ClassInfos => classContainer.ItemList;
        public IReadOnlyList<UnitInfoAI> UnitAIEntries => unitAIContainer.ItemList;
        public IReadOnlyList<MapDefinition> MapEntries => mapEntries;
        public IReadOnlyList<FactionDefinition> FactionEntries => factionEntries;
        public FactionDefinition DefaultFaction => defaultFaction;
        public UnitMovementDefinition UnitMovementDefinition => unitMovementDefinition;
        public SpellInfoContainer Spells => spellContainer;

        public void Register()
        {
            auraContainer.Register();
            classContainer.Register();
            creatureContainer.Register();
            vehicleContainer.Register();
            spellContainer.Register();
            unitAIContainer.Register();
        }

        public void Unregister()
        {
            unitAIContainer.Unregister();
            spellContainer.Unregister();
            vehicleContainer.Unregister();
            creatureContainer.Unregister();
            classContainer.Unregister();
            auraContainer.Unregister();
        }

        public bool IsStealthAura(int auraId) => auraContainer.IsStealthAura(auraId);
    }
}
