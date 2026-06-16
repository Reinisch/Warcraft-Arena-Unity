using UnityEngine;

namespace Assets.Scripts.Core
{
    [CreateAssetMenu(fileName = "Entity Prefab", menuName = "Game Data/Balance/Entity Prefab", order = 1)]
    public class WorldEntityPrefab: ScriptableObject
    {
        [field:SerializeField]
        public GameObject Prototype { get; private set; }
    }
}
