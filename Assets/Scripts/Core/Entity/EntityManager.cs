using System;
using System.Collections.Generic;
using Bolt;
using Common;

namespace Core
{
    public class EntityManager<T> where T : Entity
    {
        private readonly Dictionary<ulong, T> entitiesById = new Dictionary<ulong, T>();
        protected readonly List<T> Entities = new List<T>();

        public event Action<T> EventEntityAttached;
        public event Action<T> EventEntityDetach;

        public virtual void Dispose()
        {
            while (Entities.Count > 0)
                Destroy(Entities[0]);
        }

        public TEntity Create<TEntity>(PrefabId prefabId, Entity.CreateToken createToken = null)
        {
            return BoltNetwork.Instantiate(prefabId, createToken).GetComponent<TEntity>();
        }

        public void Attach(T entity)
        {
            Entities.Add(entity);
            entitiesById.Add(entity.Id, entity);

            if(entity.AutoScoped)
                entity.BoltEntity.SetScopeAll(true);

            EntityAttached(entity);

            EventEntityAttached?.Invoke(entity);
        }

        public void Detach(T entity)
        {
            EventEntityDetach?.Invoke(entity);

            Entities.Remove(entity);
            entitiesById.Remove(entity.Id);

            EntityDetached(entity);
        }

        public void Destroy(T entity)
        {
            entity.Detached();

            BoltNetwork.Destroy(entity.gameObject);
        }

        public T Find(ulong networkId)
        {
            return entitiesById.LookupEntry(networkId);
        }

        public bool TryGet(ulong networkId, out T entity)
        {
            return entitiesById.TryGetValue(networkId, out entity);
        }

        internal virtual void SetScope(BoltConnection connection, bool inScope)
        {
            foreach (T entity in Entities)
                entity.BoltEntity.SetScope(connection, inScope);
        }

        internal virtual void DoUpdate(int deltaTime)
        {
            for (int i = 0; i < Entities.Count; i++)
                Entities[i].DoUpdate(deltaTime);
        }

        protected virtual void EntityAttached(T entity)
        {
        }

        protected virtual void EntityDetached(T entity)
        {
        }
    }
}