using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [Serializable]
    public class MapEntry
    {
        [SerializeField, UsedImplicitly] private int id;
        [SerializeField, UsedImplicitly] private string mapName;
        [SerializeField, UsedImplicitly] private int maxPlayers = 10;
        [SerializeField, UsedImplicitly] private MapTypes mapType;
        [SerializeField, UsedImplicitly] private Expansions expansion;

        public int Id => id;
        public string MapName => mapName;
        public int MaxPlayers => maxPlayers;
        public MapTypes MapType => mapType;
        public Expansions Expansion => expansion;

        public bool IsDungeon()
        {
            return MapType == MapTypes.Instance || MapType == MapTypes.Raid;
        }
    }
}