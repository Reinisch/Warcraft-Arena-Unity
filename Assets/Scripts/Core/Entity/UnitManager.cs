using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class UnitManager : EntityManager<Unit>
    {
        private readonly List<Player> players = new List<Player>();
        private readonly Dictionary<Collider, Unit> unitsByColliders = new Dictionary<Collider, Unit>();

        public UnitManager(WorldManager worldManager) : base(worldManager)
        {
        }

        public override void Dispose()
        {
            players.Clear();

            base.Dispose();
        }

        protected override void EntityAttached(Unit entity)
        {
            base.EntityAttached(entity);

            unitsByColliders[entity.UnitCollider] = entity;

            if (entity.EntityType == EntityType.Player)
                players.Add((Player)entity);
        }

        protected override void EntityDetached(Unit entity)
        {
            base.EntityDetached(entity);

            unitsByColliders.Remove(entity.UnitCollider);

            if (entity.EntityType == EntityType.Player)
                players.Remove((Player)entity);
        }

        public override void SetScope(BoltConnection connection, bool inScope)
        {
            base.SetScope(connection, inScope);

            foreach (Player player in players)
                player.Controller.ClientMoveState?.SetScope(connection, false);
        }

        public void Accept(IUnitVisitor visitor)
        {
            foreach (var entity in Entities)
                entity.Accept(visitor);
        }

        public Unit Find(Collider unitCollider)
        {
            return unitsByColliders.LookupEntry(unitCollider);
        }
    }
}