using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Map Definition", menuName = "Game Data/World/Map Definition", order = 2)]
    public class MapDefinition : ScriptableObject
    {
        [SerializeField, UsedImplicitly] private int id;
        [SerializeField, UsedImplicitly] private bool isAvailable;
        [SerializeField, UsedImplicitly] private string mapName;
        [SerializeField, UsedImplicitly] private int maxPlayers = 10;
        [SerializeField, UsedImplicitly] private float maxVisibilityRange = 10.0f;
        [SerializeField, UsedImplicitly] private MapType mapType;
        [SerializeField, UsedImplicitly] private Expansion expansion;
        [SerializeField, UsedImplicitly] private Sprite slotBackground;

        public int Id => id;
        public bool IsAvailable => isAvailable;
        public string MapName => mapName;
        public int MaxPlayers => maxPlayers;
        public float MaxVisibilityRange => maxVisibilityRange;
        public MapType MapType => mapType;
        public Expansion Expansion => expansion;
        public Sprite SlotBackground => slotBackground;

        public bool IsDungeon()
        {
            return MapType == MapType.Instance || MapType == MapType.Raid;
        }
    }
}