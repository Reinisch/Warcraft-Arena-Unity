using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class EntityManager : SingletonGameObject<EntityManager>
    {
        [Serializable]
        private class DefaultPrototypeDefinition
        {
            [UsedImplicitly] public EntityType EntityType;
            [UsedImplicitly] public GameObject Prototype;
        }

        [SerializeField] private List<DefaultPrototypeDefinition> defaultPrototypes;

        private Dictionary<EntityType, GameObject> DefaultPrototypes { get; } = new Dictionary<EntityType, GameObject>();
        private Dictionary<Guid, WorldEntity> SpawnedEntities { get; } = new Dictionary<Guid, WorldEntity>();
        
        public void Initialize()
        {
            defaultPrototypes.ForEach(protoDefinition => DefaultPrototypes.Add(protoDefinition.EntityType, protoDefinition.Prototype));

            foreach (var spawnedEntity in SpawnedEntities)
                spawnedEntity.Value.Deinitialize();

            SpawnedEntities.Clear();
        }

        public void Deinitialize()
        {
            defaultPrototypes.Clear();
        }

        public T SpawnEntity<T>(EntityType type, EntitySpawnData spawnData) where T : WorldEntity
        {
            GameObject prototype = DefaultPrototypes.LookupEntry(type);
            Assert.IsNotNull(prototype, $"Missing prototype for entity: {type}");

            T entity = Instantiate(prototype, spawnData.Position, spawnData.Rotation).GetComponent<T>();
            Guid spawnGuid = Guid.NewGuid();
            Assert.IsFalse(SpawnedEntities.ContainsKey(spawnGuid), $"Same Guid generated for entity: {type} Guid: {spawnGuid}");

            entity.Initialize(true, spawnGuid);
            SpawnedEntities.Add(spawnGuid, entity);
            return entity;
        }

        public void DespawnEntity(WorldEntity entity)
        {
            Assert.IsTrue(SpawnedEntities.ContainsKey(entity.Guid), $"Attempted to despawn missing entity: {entity.TypeId} Guid: {entity.Guid}");

            entity.Deinitialize();
            SpawnedEntities.Remove(entity.Guid);
            Destroy(entity.gameObject);
        }
    }
}