using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class GridCell : MonoBehaviour
    {
        private readonly GridReferenceManager<Player> worldPlayers = new GridReferenceManager<Player>();
        private readonly GridReferenceManager<Creature> worldPlayerPets = new GridReferenceManager<Creature>();

        private readonly GridReferenceManager<GameEntity> gridGameEntities = new GridReferenceManager<GameEntity>();
        private readonly GridReferenceManager<DynamicEntity> gridDynamics = new GridReferenceManager<DynamicEntity>();
        private readonly GridReferenceManager<AreaTrigger> gridAreaTriggers = new GridReferenceManager<AreaTrigger>();

        public int WorldPlayerCount => worldPlayers.Count;
        public int WorldPetCount => worldPlayerPets.Count;

        private Map map;

        public void Initialize(Map map)
        {
            this.map = map;
        }

        public void Deinitialize()
        {
            worldPlayers.Invalidate();
            worldPlayerPets.Invalidate();

            gridGameEntities.Invalidate();
            gridDynamics.Invalidate();
            gridAreaTriggers.Invalidate();
        }


        public void AddWorldEntity(Player player)
        {
            player.AddToGrid(worldPlayers);
            Assert.IsTrue(player.IsInGrid());
        }

        public void AddWorldEntity(Pet creature)
        {
            creature.AddToGrid(worldPlayerPets);
            Assert.IsTrue(creature.IsInGrid());
        }

        public void AddWorldEntity<TNotWorldEntity>(TNotWorldEntity entity) where TNotWorldEntity : Entity { }


        public void AddGridEntity(GameEntity gameEntity)
        {
            gameEntity.AddToGrid(gridGameEntities);
            Assert.IsTrue(gameEntity.IsInGrid());
        }

        public void AddGridEntity(DynamicEntity dynamicEntity)
        {
            dynamicEntity.AddToGrid(gridDynamics);
            Assert.IsTrue(dynamicEntity.IsInGrid());
        }

        public void AddGridEntity(AreaTrigger areaTrigger)
        {
            areaTrigger.AddToGrid(gridAreaTriggers);
            Assert.IsTrue(areaTrigger.IsInGrid());
        }

        public void AddGridEntity<TNotGridEntity>(TNotGridEntity entity) where TNotGridEntity : Entity { }


        public void Visit(IGridVisitor visitor)
        {
            visitor.Visit(gridGameEntities);
            visitor.Visit(gridDynamics);
            visitor.Visit(gridAreaTriggers);
        }

        public void Visit(IWorldVisitor visitor)
        {
            visitor.Visit(worldPlayers);
            visitor.Visit(worldPlayerPets);
        }

        public void Visit<TEntityVisitor>(TEntityVisitor visitor) where TEntityVisitor : IEntityVisitor
        {
            visitor.Visit(worldPlayers);
            visitor.Visit(worldPlayerPets);

            visitor.Visit(gridGameEntities);
            visitor.Visit(gridDynamics);
            visitor.Visit(gridAreaTriggers);
        }
    }
}