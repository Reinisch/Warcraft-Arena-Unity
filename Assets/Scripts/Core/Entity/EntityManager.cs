using System;
using System.Collections.Generic;
using Bolt;

namespace Core
{
    public class EntityManager<T> where T : Entity
    {
        private readonly Dictionary<ulong, T> entitiesById = new Dictionary<ulong, T>();
        private readonly List<T> entities = new List<T>();

        public event Action<T> EventEntityAttached;
        public event Action<T> EventEntityDetach;

        public void Initialize()
        {
        }

        public void Deinitialize()
        {
            while (entities.Count > 0)
                Destroy(entities[0]);
        }

        public void Attach(T entity)
        {
            entities.Add(entity);
            entitiesById.Add(entity.NetworkId, entity);

            EventEntityAttached?.Invoke(entity);
        }

        public void Detach(T entity)
        {
            EventEntityDetach?.Invoke(entity);

            entities.Remove(entity);
            entitiesById.Remove(entity.NetworkId);
        }

        public void Destroy(T entity)
        {
            if (entity.BoltEntity.isAttached)
                Detach(entity);

            BoltNetwork.Destroy(entity.gameObject);
        }

        public TEntity Create<TEntity>(PrefabId prefabId, Entity.CreateInfo createInfo) where TEntity : T
        {
            return BoltNetwork.Instantiate(prefabId, createInfo).GetComponent<TEntity>();
        }

        public T Find(ulong networkId)
        {
            return entitiesById.LookupEntry(networkId);
        }
    }
}