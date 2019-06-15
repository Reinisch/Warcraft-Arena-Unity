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

        public IReadOnlyCollection<FactionDefinition> HostileFactions => hostileFactions;
        public IReadOnlyCollection<FactionDefinition> FriendlyFactions => friendlyFactions;
        public string LocalizationId => localizationId;
        public int FactionId => factionId;
        public Team Team => team;
    }
}
