using Unity.Netcode;
using UnityEngine;
using Zenject;

namespace Net.Ngo
{
    /// <summary>
    /// Instantiates a networked prefab through the Zenject container so <c>[Inject]</c> dependencies
    /// (World, Balance, registry, …) are satisfied — NGO's default instantiation skips DI, which would
    /// leave client-side Core entities un-injected. Register one per networked entity prefab via
    /// <c>NetworkManager.PrefabHandler.AddHandler(prefab, handler)</c>.
    /// </summary>
    public sealed class ZenjectNetworkPrefabInstanceHandler : INetworkPrefabInstanceHandler
    {
        private readonly GameObject prefab;
        private readonly DiContainer container;

        public ZenjectNetworkPrefabInstanceHandler(GameObject prefab, DiContainer container)
        {
            this.prefab = prefab;
            this.container = container;
        }

        public NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
        {
            // Plain Instantiate (not container.InstantiatePrefab): Zenject's prefab path calls
            // Transform.SetParent to attach the instance to the DI context, and NGO forbids re-parenting a
            // NetworkObject before it is spawned ("can only be re-parented after being spawned"). We create
            // it unparented, then inject dependencies manually so [Inject] fields are still satisfied.
            GameObject instance = Object.Instantiate(prefab, position, rotation);
            container.InjectGameObject(instance);
            return instance.GetComponent<NetworkObject>();
        }

        public void Destroy(NetworkObject networkObject)
        {
            Object.Destroy(networkObject.gameObject);
        }
    }
}
