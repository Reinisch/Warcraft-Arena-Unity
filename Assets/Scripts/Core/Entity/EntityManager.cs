using Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Core
{
    public abstract class EntityManager<T> where T : Entity
    {
        private readonly Dictionary<ulong, T> entitiesById = new Dictionary<ulong, T>();
        private Transform container;

        public readonly List<T> Entities = new List<T>();

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

        public TEntity FindNearby<TEntity>(Vector3 position, Predicate<TEntity> predicate = null) where TEntity : WorldEntity
        {
            float minDistanceSqr = float.MaxValue;
            TEntity nearbyEntity = null;

            foreach(T enity in Entities)
            {
                if (enity is not TEntity targetEntity || (predicate != null && !predicate(targetEntity)))
                    continue;

                float distanceSqr = (position - targetEntity.Position).sqrMagnitude;
                if (distanceSqr < minDistanceSqr)
                {
                    minDistanceSqr = distanceSqr;
                    nearbyEntity = targetEntity;
                }
            }

            return nearbyEntity;
        }

        public TEntity FindRandom<TEntity>(Predicate<TEntity> predicate = null)
        {
            List<TEntity> targets = new();
            foreach (T enity in Entities)
            {
                if (enity is not TEntity targetEntity || (predicate != null && !predicate(targetEntity)))
                    continue;

                targets.Add(targetEntity);
            }

            if (targets.Count > 0)
                return RandomUtils.GetRandomElement(targets);

            return default;
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