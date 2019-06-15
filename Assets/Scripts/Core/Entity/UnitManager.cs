using System.Collections.Generic;
using Common;
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

        public override void SetScope(BoltConnection connection, bool inScope)
        {
            base.SetScope(connection, inScope);

            foreach (Player player in players)
                player.Controller.ClientMoveState?.SetScope(connection, false);
        }

        public Unit Find(Collider unitCollider)
        {
            return unitsByColliders.LookupEntry(unitCollider);
        }
    }
}