using System;
using System.Collections.Generic;
using Bolt;

namespace Core
{
    public class EntityManager<T> where T : Entity
    {
        protected readonly Dictionary<ulong, T> entitiesById = new Dictionary<ulong, T>();
        protected readonly List<T> entities = new List<T>();
        protected readonly WorldManager worldManager;

        public event Action<T> EventEntityAttached;
        public event Action<T> EventEntityDetach;

        protected EntityManager(WorldManager worldManager)
        {
            this.worldManager = worldManager;
        }

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
            if (entity.BoltEntity.isAttached)
                Detach(entity);

            BoltNetwork.Destroy(entity.gameObject);
        }

        public void SetScope(BoltConnection connection, bool inScope)
        {
            foreach (T entity in entities)
                entity.BoltEntity.SetScope(connection, inScope);
        }

        public void Accept(IVisitor visitor)
        {
            foreach (var entity in entities)
                entity.Accept(visitor);
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