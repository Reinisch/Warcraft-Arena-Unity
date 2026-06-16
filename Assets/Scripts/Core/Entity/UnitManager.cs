using Assets.Scripts.Core;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Core
{
    public class UnitManager : EntityManager<Unit>
    {
        [Inject]
        private WorldEntityFactory entityFactory;

        private readonly Dictionary<Collider, Unit> unitsByColliders = new();

        public bool TryFind(Collider unitCollider, out Unit entity)
        {
            return unitsByColliders.TryGetValue(unitCollider, out entity);
        }

        public void DestroyMapUnits(Map map)
        {
            for (int i = Entities.Count - 1; i >= 0; i--)
            {
                Unit unit = Entities[i];
                if (unit.Map == map)
                    Destroy(unit);
            }
        }

        public TEntity Create<TEntity>(WorldEntityPrefab prefab, Entity.CreateToken createToken) where TEntity : Unit
        {
            return entityFactory.Create<TEntity>(prefab, createToken).GetComponent<TEntity>();
        }

        protected override void EntityAttached(Unit entity)
        {
            base.EntityAttached(entity);

            unitsByColliders[entity.UnitCollider] = entity;
        }

        protected override void EntityDetached(Unit entity)
        {
            base.EntityDetached(entity);

            unitsByColliders.Remove(entity.UnitCollider);
        }
    }
}