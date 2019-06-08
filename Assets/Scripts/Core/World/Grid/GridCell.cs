using System;
using UnityEngine;
using Common;

namespace Core
{
    public class GridCell
    {
        private readonly GridReferenceManager<Player> worldPlayers = new GridReferenceManager<Player>();
        private readonly GridReferenceManager<Creature> worldCreatures = new GridReferenceManager<Creature>();

        private int xIndex;
        private int zIndex;
        private Bounds bounds;

        public Bounds Bounds => bounds;

        internal void Initialize(Map map, int xIndex, int zIndex, Bounds bounds)
        {
            this.xIndex = xIndex;
            this.zIndex = zIndex;
            this.bounds = bounds;
        }

        internal void Deinitialize()
        {
            worldPlayers.Clear();
            worldCreatures.Clear();
        }

        internal void AddWorldEntity(WorldEntity worldEntity)
        {
            worldEntity.CurrentCell = this;

            switch (worldEntity)
            {
                case Creature creature:
                    creature.GridRef.Link(creature, worldCreatures);
                    break;
                case Player player:
                    player.GridReference.Link(player, worldPlayers);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(worldEntity));
            }
        }

        internal void RemoveWorldEntity(WorldEntity worldEntity)
        {
            worldEntity.CurrentCell = null;

            switch (worldEntity)
            {
                case Creature creature:
                    Assert.IsTrue(worldCreatures.Contains(creature.GridRef));
                    creature.GridRef.Invalidate();
                    break;
                case Player player:
                    Assert.IsTrue(worldPlayers.Contains(player.GridReference));
                    player.GridReference.Invalidate();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(worldEntity));
            }
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