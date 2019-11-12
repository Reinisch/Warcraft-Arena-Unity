using System.Collections.Generic;
using UnityEngine;
using Common;

namespace Core
{
    public class WorldGrid
    {
        private class GridCellRelocator : IUnitVisitor
        {
            private readonly WorldGrid worldGrid;

            public GridCellRelocator(WorldGrid worldGrid)
            {
                this.worldGrid = worldGrid;
            }

            public void Visit(Player entity)
            {
                VisitUnit(entity);
            }

            public void Visit(Creature entity)
            {
                VisitUnit(entity);
            }

            private bool IsOutOfCellBounds(Vector3 position, GridCell cell)
            {
                if (position.x + MovementUtils.GridCellSwitchDifference < cell.MinBounds.x)
                {
                    return true;
                }

                if (position.x > cell.MaxBounds.x + MovementUtils.GridCellSwitchDifference)
                {
                    return true;
                }

                if (position.z + MovementUtils.GridCellSwitchDifference < cell.MinBounds.z)
                {
                    return true;
                }

                if (position.z > cell.MaxBounds.z + MovementUtils.GridCellSwitchDifference)
                {
                    return true;
                }

                return false;
            }

            private void VisitUnit(Unit unit)
            {
                if (unit.Position.y > MovementUtils.MaxHeight || unit.Position.y < MovementUtils.MinHeight)
                {
                    unit.Position = worldGrid.map.Settings.DefaultSpawnPoint.position;
                }

                if (IsOutOfCellBounds(unit.Position, unit.CurrentCell))
                {
                    worldGrid.relocatableEntities.Add(unit);
                }
            }
        }

        private readonly List<WorldEntity> relocatableEntities = new List<WorldEntity>();
        private GridCellRelocator gridCellRelocator;
        private GridCell[,] cells;
        private GridCell invalidCell;
        private int cellCountX;
        private int cellCountZ;
        private float gridCellSize;
        private Map map;

        internal void Initialize(Map map)
        {
            this.map = map;
            gridCellRelocator = new GridCellRelocator(this);
            gridCellSize = map.Settings.GridCellSize;

            cellCountX = Mathf.CeilToInt(map.Settings.BoundingBox.bounds.size.x / gridCellSize);
            cellCountZ = Mathf.CeilToInt(map.Settings.BoundingBox.bounds.size.z / gridCellSize);

            cells = new GridCell[cellCountX, cellCountZ];
            for (int i = 0; i < cellCountX; i++)
                for (int j = 0; j < cellCountZ; j++)
                    cells[i, j] = new GridCell();

            Vector3 minBounds = map.Settings.BoundingBox.bounds.min;
            Vector3 origin = new Vector3(minBounds.x, map.Settings.BoundingBox.center.y, minBounds.z);
            Vector3 cellSize = new Vector3(gridCellSize, 1.0f, gridCellSize);
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

            if (startingCell == null)
            {
                entity.Position = map.Settings.DefaultSpawnPoint.position;
                startingCell = FindCell(entity.Position);
            }

            Assert.IsNotNull(startingCell, $"Starting cell is not found for {entity.GetType()} at {entity.Position}");
            startingCell.AddWorldEntity(entity);
        }

        internal void RemoveEntity(WorldEntity entity)
        {
            Assert.IsNotNull(entity.CurrentCell, $"Cell is missing on removal for {entity.GetType()} at {entity.Position}");
            entity.CurrentCell.RemoveWorldEntity(entity);
        }

        public void VisitInRadius(WorldEntity referer, float radius, IUnitVisitor unitVisitor)
        {
            int cellRange = Mathf.CeilToInt(radius / gridCellSize);
            GridCell originCell = referer.CurrentCell;
            if (originCell == null)
                return;

            int minX = Mathf.Clamp(originCell.X - cellRange, 0, cellCountX - 1);
            int maxX = Mathf.Clamp(originCell.X + cellRange, 0, cellCountX - 1);
            int minZ = Mathf.Clamp(originCell.Z - cellRange, 0, cellCountZ - 1);
            int maxZ = Mathf.Clamp(originCell.Z + cellRange, 0, cellCountZ - 1);

            for (int i = minX; i <= maxX; i++)
            for (int j = minZ; j <= maxZ; j++)
            {
                Drawing.DrawLine(referer.Position + Vector3.up, cells[i, j].Center + Vector3.up * 20, Color.red, 1.0f);
                Drawing.DrawLine(cells[i, j].MaxBounds + Vector3.up * 20, cells[i, j].MinBounds + Vector3.up * 20, cells[i, j] != originCell ? Color.green : Color.yellow, 1.0f);

                cells[i, j].Visit(unitVisitor);
            }
        }

        private GridCell FindCell(Vector3 position)
        {
            Vector3 offset = position - cells[0, 0].MinBounds;
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