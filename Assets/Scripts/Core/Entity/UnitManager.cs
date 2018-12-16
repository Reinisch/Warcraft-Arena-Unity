using System.Collections.Generic;

namespace Core
{
    public class UnitManager : EntityManager<Unit>
    {
        protected readonly List<Player> players = new List<Player>();

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

            if (entity.EntityType == EntityType.Player)
                players.Add((Player)entity);
        }

        protected override void EntityDetached(Unit entity)
        {
            base.EntityDetached(entity);

            if (entity.EntityType == EntityType.Player)
                players.Remove((Player)entity);
        }

        public void Accept(IUnitVisitor visitor)
        {
            foreach (var entity in entities)
                entity.Accept(visitor);
        }
    }
}