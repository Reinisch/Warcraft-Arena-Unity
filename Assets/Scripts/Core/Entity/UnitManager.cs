using System.Collections.Generic;
using Bolt;
using UnityEngine;

namespace Core
{
    public class UnitManager : EntityManager<Unit>
    {
        private readonly Dictionary<Collider, Unit> unitsByColliders = new Dictionary<Collider, Unit>();

        public bool TryFind(Collider unitCollider, out Unit entity)
        {
            return unitsByColliders.TryGetValue(unitCollider, out entity);
        }

        internal TEntity Create<TEntity>(PrefabId prefabId, Entity.CreateToken createToken = null) where TEntity : Unit
        {
            TEntity entity = BoltNetwork.Instantiate(prefabId, createToken).GetComponent<TEntity>();
            entity.ModifyDeathState(DeathState.Alive);
            entity.Attributes.SetHealth(entity.MaxHealth);
            entity.Motion.UpdateMovementControl(true);
            return entity;
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