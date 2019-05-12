using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Core
{
    public class WorldGrid
    {
        private class GridCellRelocator : IWorldEntityGridVisitor
        {
            private readonly WorldGrid worldGrid;

            public GridCellRelocator(WorldGrid worldGrid)
            {
                this.worldGrid = worldGrid;
            }
            
            private bool IsOutOfCellBounds(Vector3 position, GridCell cell)
            {
                if (position.x + GridHelper.GridCellSwitchDifference < cell.Bounds.min.x)
                {
                    return true;
                }

                if (position.x > cell.Bounds.max.x + GridHelper.GridCellSwitchDifference)
                {
                    return true;
                }

                if (position.z + GridHelper.GridCellSwitchDifference < cell.Bounds.min.z)
                {
                    return true;
                }

                if (position.z > cell.Bounds.max.z + GridHelper.GridCellSwitchDifference)
                {
                    return true;
                }

                return false;
            }

            public void Visit<TEntity>(GridReferenceManager<TEntity> container) where TEntity : WorldEntity
            {
                GridReference<TEntity> reference = container.FirstReference;
                while (reference != null)
                {
                    if (reference.Target.Position.y > GridHelper.MaxHeight || reference.Target.Position.y < GridHelper.MinHeight)
                    {
                        reference.Target.Position = worldGrid.map.Settings.DefaultSpawnPoint.position;
                    }

                    if (IsOutOfCellBounds(reference.Target.Position, reference.Target.CurrentCell))
                    {
                        worldGrid.relocatableEntities.Add(reference.Target);
                    }

                    reference = reference.Next;
                }
            }
        }

        private readonly List<WorldEntity> relocatableEntities = new List<WorldEntity>();
        private GridCellRelocator gridCellRelocator;
        private GridCell[,] cells;
        private GridCell invalidCell;
        private int cellCountX;
        private int cellCountZ;
        private Map map;

        internal void Initialize(Map map)
        {
            this.map = map;
            gridCellRelocator = new GridCellRelocator(this);
            float gridCellSize = map.Settings.GridCellSize;

            cellCountX = Mathf.CeilToInt(map.Settings.BoundingBox.bounds.size.x / gridCellSize);
            cellCountZ = Mathf.CeilToInt(map.Settings.BoundingBox.bounds.size.z / gridCellSize);

            cells = new GridCell[cellCountX, cellCountZ];
            for (int i = 0; i < cellCountX; i++)
                for (int j = 0; j < cellCountZ; j++)
                    cells[i, j] = new GridCell();

            Vector3 origin = map.Settings.BoundingBox.bounds.min;
            Vector3 cellSize = new Vector3(gridCellSize, map.Settings.BoundingBox.size.y, gridCellSize);
            for (int i = 0; i < cellCountX; i++)
            {
                float xOffset = (i + 0.5f) * gridCellSize;
                for (int j = 0; j < cellCountZ; j++)
                {
                    float zOffset = (j + 0.5f) * gridCellSize;
                    Vector3 cellCenter = origin + new Vector3(xOffset, 0.0f, zOffset);
                    cells[i, j].Initialize(map, i, j, new Bounds(cellCenter, cellSize));
                }
            }

            invalidCell = new GridCell();
            invalidCell.Initialize(map, -1, -1, new Bounds(Vector3.zero, Vector3.positiveInfinity));
        }

        internal void Deinitialize()
        {
            invalidCell.Deinitialize();

            for (int i = 0; i < cellCountX; i++)
                for (int j = 0; j < cellCountZ; j++)
                    cells[i, j].Deinitialize();
        }

        internal void DoUpdate(int deltaTime)
        {
            for (int i = 0; i < cellCountX; i++)
                for (int j = 0; j < cellCountZ; j++)
                    cells[i, j].Visit(gridCellRelocator);

            foreach (WorldEntity relocatableEntity in relocatableEntities)
            {
                relocatableEntity.CurrentCell.RemoveWorldEntity(relocatableEntity);
                GridCell nextCell = FindCell(relocatableEntity.Position);
                if (nextCell == null)
                {
                    relocatableEntity.Position = map.Settings.DefaultSpawnPoint.position;
                    nextCell = FindCell(relocatableEntity.Position);
                }

                nextCell.AddWorldEntity(relocatableEntity);
            }

            relocatableEntities.Clear();
        }

        internal void AddEntity(WorldEntity entity)
        {
            GridCell startingCell = FindCell(entity.Position);
            Assert.IsNotNull(startingCell, $"Starting cell is not found for {entity.GetType()} at {entity.Position}");

            if (startingCell == null)
            {
                entity.Position = map.Settings.DefaultSpawnPoint.position;
                startingCell = FindCell(entity.Position);
            }

            startingCell.AddWorldEntity(entity);
        }

        internal void RemoveEntity(WorldEntity entity)
        {
            Assert.IsNotNull(entity.CurrentCell, $"Cell is missing on removal for {entity.GetType()} at {entity.Position}");
            entity.CurrentCell.RemoveWorldEntity(entity);
        }

        public void VisitAllGrids(IEntityGridVisitor gridVisitor)
        {
            for (int i = 0; i < cellCountX; i++)
                for (int j = 0; j < cellCountZ; j++)
                    cells[i, j].Visit(gridVisitor);
        }

        public void VisitGrid(int x, int z, IEntityGridVisitor gridVisitor)
        {
            Assert.IsTrue(x < cellCountX && z < cellCountZ);
            cells[x, z].Visit(gridVisitor);
        }

        private GridCell FindCell(Vector3 position)
        {
            Vector3 offset = position - cells[0, 0].Bounds.min;
            int xCell = Mathf.FloorToInt(offset.x / map.Settings.GridCellSize);
            int zCell = Mathf.FloorToInt(offset.z / map.Settings.GridCellSize);

            if (xCell < 0 || xCell > cellCountX - 1 || zCell < 0 || zCell > cellCountZ - 1)
            {
                return null;
            }

            return cells[xCell, zCell];
        }
    }
}