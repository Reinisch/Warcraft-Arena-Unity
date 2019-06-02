using System;
using System.Collections.Generic;
using Bolt;

namespace Core
{
    public class EntityManager<T> where T : Entity
    {
        protected readonly Dictionary<ulong, T> EntitiesById = new Dictionary<ulong, T>();
        protected readonly List<T> Entities = new List<T>();
        protected readonly WorldManager WorldManager;

        public event Action<T> EventEntityAttached;
        public event Action<T> EventEntityDetach;

        protected EntityManager(WorldManager worldManager)
        {
            WorldManager = worldManager;
        }

        public virtual void Dispose()
        {
            while (Entities.Count > 0)
                Destroy(Entities[0]);
        }

        public TEntity Create<TEntity>(PrefabId prefabId, Entity.CreateInfo createInfo = null)
        {
            return BoltNetwork.Instantiate(prefabId, createInfo).GetComponent<TEntity>();
        }

        public void Attach(T entity)
        {
            Entities.Add(entity);
            EntitiesById.Add(entity.NetworkId, entity);

            if(entity.AutoScoped)
                entity.BoltEntity.SetScopeAll(true);

            EntityAttached(entity);

            EventEntityAttached?.Invoke(entity);
        }

        public void Detach(T entity)
        {
            EventEntityDetach?.Invoke(entity);

            Entities.Remove(entity);
            EntitiesById.Remove(entity.NetworkId);

            EntityDetached(entity);
        }

        public void Destroy(T entity)
        {
            if (entity.BoltEntity.isAttached)
                Detach(entity);

            BoltNetwork.Destroy(entity.gameObject);
        }

        public virtual void SetScope(BoltConnection connection, bool inScope)
        {
            foreach (T entity in Entities)
                entity.BoltEntity.SetScope(connection, inScope);
        }

        public void Accept(IVisitor visitor)
        {
            foreach (var entity in Entities)
                entity.Accept(visitor);
        }

        public T Find(ulong networkId)
        {
            return EntitiesById.LookupEntry(networkId);
        }

        protected virtual void EntityAttached(T entity)
        {
        }

        protected virtual void EntityDetached(T entity)
        {
        }
    }
}