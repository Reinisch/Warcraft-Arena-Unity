using System;
using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Core
{
    internal sealed partial class MapGrid
    {
        internal sealed class Cell
        {
            private readonly List<Player> worldPlayers = new List<Player>();
            private readonly List<Creature> worldCreatures = new List<Creature>();
            private readonly Bounds bounds;

            public Vector3 Center { get; }
            public Vector3 MaxBounds { get; }
            public Vector3 MinBounds { get; }
            public int X { get; }
            public int Z { get; }

            internal Cell(int xIndex, int zIndex, Bounds bounds)
            {
                X = xIndex;
                Z = zIndex;

                this.bounds = bounds;

                Center = bounds.center;
                MaxBounds = bounds.max;
                MinBounds = bounds.min;
            }

            internal void Dispose()
            {
                worldPlayers.Clear();
                worldCreatures.Clear();
            }

            internal void AddWorldEntity(WorldEntity worldEntity)
            {
                worldEntity.CurrentCell = this;
                worldEntity.IsVisibilityChanged = true;

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

            internal void Visit(IUnitVisitor unitVisitor)
            {
                for (var i = 0; i < worldPlayers.Count; i++)
                    worldPlayers[i].Accept(unitVisitor);

                for (var i = 0; i < worldCreatures.Count; i++)
                    worldCreatures[i].Accept(unitVisitor);
            }

            public override string ToString() => $"X:{X} Z:{Z} Bounds:{bounds}";
        }
    }
}