using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Faction Definition", menuName = "Game Data/Gameplay/Faction Definition", order = 1)]
    public class FactionDefinition : ScriptableObject
    {
        [UsedImplicitly, SerializeField] private string localizationId;
        [UsedImplicitly, SerializeField] private int factionId;
        [UsedImplicitly, SerializeField] private Team team;
        [UsedImplicitly, SerializeField] private List<FactionDefinition> hostileFactions;
        [UsedImplicitly, SerializeField] private List<FactionDefinition> friendlyFactions;

        public HashSet<FactionDefinition> HostileFactions { get; } = new HashSet<FactionDefinition>();
        public HashSet<FactionDefinition> FriendlyFactions { get; } = new HashSet<FactionDefinition>();
        public string LocalizationId => localizationId;
        public int FactionId => factionId;
        public Team Team => team;

        [UsedImplicitly]
        private void OnEnable()
        {
            HostileFactions.Clear();
            FriendlyFactions.Clear();

            foreach (FactionDefinition hostileFaction in hostileFactions)
                HostileFactions.Add(hostileFaction);

            foreach (FactionDefinition friendlyFaction in friendlyFactions)
                FriendlyFactions.Add(friendlyFaction);
        }

        [UsedImplicitly]
        private void OnValidate()
        {
            OnEnable();
        }
    }
}
