using UnityEngine;
using Zenject;

namespace Common
{
    public class GameObjectFactory
    {
        [Inject]
        private DiContainer diContainer;

        public TComponent Create<TComponent>(GameObject prefab, Transform parent) where TComponent : Component
        {
            return diContainer.InstantiatePrefab(prefab, parent).GetComponent<TComponent>();
        }

        public GameObject Create(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent)
        {
            return diContainer.InstantiatePrefab(prefab, position, rotation, parent);
        }

        public TComponent Create<TComponent>(TComponent prefabComponent, Transform parent) where TComponent : Component
        {
            return diContainer.InstantiatePrefab(prefabComponent.gameObject, parent).GetComponent<TComponent>();
        }
    }
}