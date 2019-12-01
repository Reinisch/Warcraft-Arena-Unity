using System;
using System.Collections.Generic;
using Common;
using UnityEngine;

namespace Core
{
    public abstract class EntityManager<T> where T : Entity
    {
        private readonly Dictionary<ulong, T> entitiesById = new Dictionary<ulong, T>();
        protected readonly List<T> Entities = new List<T>();
        private Transform container;

        public event Action<T> EventEntityAttached;
        public event Action<T> EventEntityDetach;

        protected EntityManager()
        {
            container = GameObject.FindGameObjectWithTag("Entity Container").transform;
        }

        public virtual void Dispose()
        {
            while (Entities.Count > 0)
                Destroy(Entities[0]);

            container = null;
        }

        public void Attach(T entity)
        {
            entity.transform.SetParent(container);
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

        public bool TryFind(ulong networkId, out T entity)
        {
            return entitiesById.TryGetValue(networkId, out entity);
        }

        internal virtual void SetDefaultScope(BoltConnection connection)
        {
            foreach (T entity in Entities)
                entity.BoltEntity.SetScope(connection, entity.AutoScoped);
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