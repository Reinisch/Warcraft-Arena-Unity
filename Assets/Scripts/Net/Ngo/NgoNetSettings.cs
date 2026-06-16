using UnityEngine;

namespace Net.Ngo
{
    /// <summary>
    /// References the network shadow prefabs (separate NetworkObjects that mirror the Core units).
    /// Assigned in the inspector and bound in DI so the controller can register prefab handlers and the
    /// spawner can pick the right shadow per unit kind.
    /// </summary>
    [CreateAssetMenu(fileName = "NGO Net Settings", menuName = "Game Data/Net/NGO Net Settings")]
    public sealed class NgoNetSettings : ScriptableObject
    {
        [SerializeField] private GameObject playerShadowPrefab;
        [SerializeField] private GameObject creatureShadowPrefab;

        public GameObject PlayerShadowPrefab => playerShadowPrefab;
        public GameObject CreatureShadowPrefab => creatureShadowPrefab;
    }
}
