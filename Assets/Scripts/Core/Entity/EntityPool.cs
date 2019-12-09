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
        private World world;

        public void Initialize(World world)
        {
            this.world = world;
        }

        public void Deinitialize()
        {
            takenEntities.Clear();

            world = null;
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

                EventHandler.ExecuteEvent(gameObject, GameEvents.EntityPooled, true, world);
                takenEntities.Add(gameObject, createdEntity);
            }

            return gameObject;
        }

        public void Destroy(GameObject gameObject)
        {
            if (!gameObject.CompareTag("Move State"))
            {
                Assert.IsTrue(takenEntities.ContainsKey(gameObject), $"Trying to destroy pooled entity, but pool has no such object: {gameObject.name}");

                EventHandler.ExecuteEvent(gameObject, GameEvents.EntityPooled, false, world);
                takenEntities.Remove(gameObject);
            }

            GameObjectPool.Return(gameObject, false);
        }
    }
}
