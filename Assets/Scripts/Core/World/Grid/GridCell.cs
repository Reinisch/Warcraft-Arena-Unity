using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class GridCell
    {
        private readonly GridReferenceManager<Player> worldPlayers = new GridReferenceManager<Player>();
        private readonly GridReferenceManager<Creature> worldCreatures = new GridReferenceManager<Creature>();

        private Map map;
        private int xIndex;
        private int zIndex;
        private Bounds bounds;

        public Bounds Bounds => bounds;

        internal void Initialize(Map map, int xIndex, int zIndex, Bounds bounds)
        {
            this.map = map;
            this.xIndex = xIndex;
            this.zIndex = zIndex;
            this.bounds = bounds;
        }

        internal void Deinitialize()
        {
            worldPlayers.Clear();
            worldCreatures.Clear();

            map = null;
        }

        internal void AddWorldEntity(WorldEntity worldEntity)
        {
            worldEntity.CurrentCell = this;
            Debug.Log($"Entity {worldEntity.NetworkId} moved to {this}");

            switch (worldEntity)
            {
                case Creature creature:
                    creature.GridRef.Link(creature, worldCreatures);
                    break;
                case Player player:
                    player.GridRef.Link(player, worldPlayers);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(worldEntity));
            }
        }

        internal void RemoveWorldEntity(WorldEntity worldEntity)
        {
            switch (worldEntity)
            {
                case Creature creature:
                    Assert.IsTrue(worldCreatures.Contains(creature.GridRef));
                    creature.GridRef.Invalidate();
                    break;
                case Player player:
                    Assert.IsTrue(worldPlayers.Contains(player.GridRef));
                    player.GridRef.Invalidate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(worldEntity));
            }
        }

        public void Visit<TEntityVisitor>(TEntityVisitor visitor) where TEntityVisitor : IEntityGridVisitor
        {
            visitor.Visit(worldPlayers);
            visitor.Visit(worldCreatures);
        }

        public void Visit(IWorldEntityGridVisitor visitor)
        {
            visitor.Visit(worldPlayers);
            visitor.Visit(worldCreatures);
        }

        public override string ToString()
        {
            return $"X:{xIndex} Z:{zIndex} Bounds:{bounds}";
        }
    }
}