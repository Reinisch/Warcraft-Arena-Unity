using System;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Core
{
    public class GridCell
    {
        private readonly List<Player> worldPlayers = new List<Player>();
        private readonly List<Creature> worldCreatures = new List<Creature>();
        private Bounds bounds;

        public Vector3 Center { get; private set; }
        public Vector3 MaxBounds { get; private set; }
        public Vector3 MinBounds { get; private set; }
        public int X { get; private set; }
        public int Z { get; private set; }

        internal void Initialize(Map map, int xIndex, int zIndex, Bounds bounds)
        {
            X = xIndex;
            Z = zIndex;

            this.bounds = bounds;

            Center = bounds.center;
            MaxBounds = bounds.max;
            MinBounds = bounds.min;
        }

        internal void Deinitialize()
        {
            worldPlayers.Clear();
            worldCreatures.Clear();
        }

        internal void AddWorldEntity(WorldEntity worldEntity)
        {
            worldEntity.CurrentCell = this;
            worldEntity.VisibilityChanged = true;

            switch (worldEntity)
            {
                case Creature creature:
                    worldCreatures.Add(creature);
                    break;
                case Player player:
                    worldPlayers.Add(player);
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
                    Assert.IsTrue(worldCreatures.Contains(creature));
                    worldCreatures.Remove(creature);
                    break;
                case Player player:
                    Assert.IsTrue(worldPlayers.Contains(player));
                    worldPlayers.Remove(player);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(worldEntity));
            }
        }

        public void Visit(IUnitVisitor unitVisitor)
        {
            for (var i = 0; i < worldPlayers.Count; i++)
                worldPlayers[i].Accept(unitVisitor);

            for (var i = 0; i < worldCreatures.Count; i++)
                worldCreatures[i].Accept(unitVisitor);
        }

        public override string ToString()
        {
            return $"X:{X} Z:{Z} Bounds:{bounds}";
        }
    }
}