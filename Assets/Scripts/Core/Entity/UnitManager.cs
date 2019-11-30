using System.Collections.Generic;
using Bolt;
using UnityEngine;

namespace Core
{
    public class UnitManager : EntityManager<Unit>
    {
        private readonly List<Player> players = new List<Player>();
        private readonly Dictionary<Collider, Unit> unitsByColliders = new Dictionary<Collider, Unit>();

        public override void Dispose()
        {
            players.Clear();

            base.Dispose();
        }

        public bool TryFind(Collider unitCollider, out Unit entity)
        {
            return unitsByColliders.TryGetValue(unitCollider, out entity);
        }

        internal TEntity Create<TEntity>(PrefabId prefabId, Entity.CreateToken createToken = null) where TEntity : Unit
        {
            TEntity entity = BoltNetwork.Instantiate(prefabId, createToken).GetComponent<TEntity>();
            entity.ModifyDeathState(DeathState.Alive);
            entity.Attributes.SetHealth(entity.MaxHealth);
            entity.MovementInfo.HasMovementControl = true;
            return entity;
        }

        internal override void SetDefaultScope(BoltConnection connection)
        {
            base.SetDefaultScope(connection);

            foreach (Player player in players)
                player.MovementInfo.MoveEntity?.SetScope(connection, false);
        }

        protected override void EntityAttached(Unit entity)
        {
            base.EntityAttached(entity);

            unitsByColliders[entity.UnitCollider] = entity;

            if (entity is Player player)
                players.Add(player);
        }

        protected override void EntityDetached(Unit entity)
        {
            base.EntityDetached(entity);

            unitsByColliders.Remove(entity.UnitCollider);

            if (entity is Player player)
                players.Remove(player);
        }
    }
}