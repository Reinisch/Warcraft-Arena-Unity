using System.Collections.Generic;
using Bolt;
using Common;
using UnityEngine;

using Assert = Common.Assert;

namespace Core
{
    internal class EntityPool : IPrefabPool
    {
        private readonly Dictionary<GameObject, Entity> takenEntities = new Dictionary<GameObject, Entity>();
        private WorldManager worldManager;

        public void Initialize(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

        public void Deinitialize()
        {
            takenEntities.Clear();

            worldManager = null;
        }

        public GameObject LoadPrefab(PrefabId prefabId)
        {
            return PrefabDatabase.Find(prefabId);
        }

        public GameObject Instantiate(PrefabId prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject gameObject = GameObjectPool.Take(LoadPrefab(prefabId), position, rotation);

            if (!gameObject.CompareTag("Move State"))
            {
                Entity createdEntity = gameObject.GetComponent<Entity>();

                createdEntity.TakenFromPool(worldManager);
                takenEntities.Add(gameObject, createdEntity);
            }

            gameObject.transform.SetParent(null);
            return gameObject;
        }

        public void Destroy(GameObject gameObject)
        {
            if (!gameObject.CompareTag("Move State"))
            {
                Assert.IsTrue(takenEntities.ContainsKey(gameObject), $"Trying to destroy pooled entity, but pool has no such object: {gameObject.name}");

                takenEntities[gameObject].ReturnedToPool();
                takenEntities.Remove(gameObject);
            }

            GameObjectPool.Return(gameObject, false);
        }
    }
}
