using System;
using System.Collections.Generic;
using Bolt;
using Common;

namespace Core
{
    public class EntityManager<T> where T : Entity
    {
        private readonly Dictionary<ulong, T> entitiesById = new Dictionary<ulong, T>();
        private readonly List<T> entities = new List<T>();

        public event Action<T> EventEntityAttached;
        public event Action<T> EventEntityDetach;

        public virtual void Dispose()
        {
            while (entities.Count > 0)
                Destroy(entities[0]);
        }

        public TEntity Create<TEntity>(PrefabId prefabId, Entity.CreateInfo createInfo = null)
        {
            return BoltNetwork.Instantiate(prefabId, createInfo).GetComponent<TEntity>();
        }

        public void Attach(T entity)
        {
            entities.Add(entity);
            entitiesById.Add(entity.NetworkId, entity);

            if(entity.AutoScoped)
                entity.BoltEntity.SetScopeAll(true);

            EntityAttached(entity);

            EventEntityAttached?.Invoke(entity);
        }

        public void Detach(T entity)
        {
            EventEntityDetach?.Invoke(entity);

            entities.Remove(entity);
            entitiesById.Remove(entity.NetworkId);

            EntityDetached(entity);
        }

        public void Destroy(T entity)
        {
            entity.Detached();

            BoltNetwork.Destroy(entity.gameObject);
        }

        public virtual void SetScope(BoltConnection connection, bool inScope)
        {
            foreach (T entity in entities)
                entity.BoltEntity.SetScope(connection, inScope);
        }

        public T Find(ulong networkId)
        {
            return entitiesById.LookupEntry(networkId);
        }

        protected virtual void EntityAttached(T entity)
        {
        }

        protected virtual void EntityDetached(T entity)
        {
        }
    }
}