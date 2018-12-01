﻿using System;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Core
{
    public class WorldGrid
    {
        private GridCell[,] cells;
        private Map map;

        public bool EntityDataLoaded { get; set; }
        public GridStateType State { get; set; }

        public GridInfo Info { get; private set; }
        public int CellCountX { get; private set; }
        public int CellCountZ { get; private set; }

        public bool UnloadLock => Info.UnloadLock;
        public bool UnloadExplicitLock { get { return Info.UnloadExplicitLock; } set { Info.UnloadExplicitLock = value; } }
        public bool UnloadReferenceLock { get { return Info.UnloadReferenceLock; } set { Info.UnloadReferenceLock = value; } }
        public int WorldPlayerCount { get { return cells.EntityCount(CellCountX, CellCountZ, cell => cell.WorldPlayerCount); } }
        public int WorldPetCount { get { return cells.EntityCount(CellCountX, CellCountZ, cell => cell.WorldPetCount); } }
        public TimeTracker TimeTracker => Info.Timer;

        public void Initialize(Map map, long expiry, bool unload = true)
        {
            Info = new GridInfo(expiry, unload);
            State = GridStateType.Invalid;

            EntityDataLoaded = false;
            CellCountX = map.Settings.GridLayout.constraintCount;
            CellCountZ = map.Settings.GridCells.Count / CellCountX;

            this.map = map;
            cells = new GridCell[CellCountX, CellCountZ];
            for (int i = 0; i < map.Settings.GridCells.Count; i++)
                cells[i % CellCountX, i / CellCountX] = map.Settings.GridCells[i];

            for (int i = 0; i < CellCountX; i++)
                for(int j = 0; j < CellCountZ; j++)
                    cells[i, j].Initialize(map);
        }

        public void Deinitialize()
        {
            for (int i = 0; i < CellCountX; i++)
                for (int j = 0; j < CellCountZ; j++)
                    cells[i, j].Deinitialize();
        }

        public GridCell GetGridType(int x, int z)
        {
            Assert.IsTrue(x < CellCountX && z < CellCountZ);
            return cells[x, z];
        }

        public void IncUnloadActiveLock() { Info.IncUnloadActiveLock(); }

        public void DecUnloadActiveLock() { Info.DecUnloadActiveLock(); }

        public void ResetTimeTracker(long interval) { Info.ResetTimeTracker(interval); }

        public void UpdateTimeTracker(long diff) { Info.UpdateTimeTracker(diff); }

    
        public void VisitAllGrids(IEntityVisitor visitor)
        {
            for (int i = 0; i < CellCountX; i++)
                for (int j = 0; j < CellCountZ; j++)
                    GetGridType(i, j).Visit(visitor);
        }

        public void VisitGrid(int x, int y, IEntityVisitor visitor)
        {
            Assert.IsTrue(x < CellCountX && y < CellCountZ);
            GetGridType(x, y).Visit(visitor);
        }
    }

    public static class GridCellExtensions
    {
        public static int EntityCount(this GridCell[,] cells, int cellSizeX, int cellSizeZ, Func<GridCell, int> selector)
        {
            int count = 0;
            for (int x = 0; x < cellSizeX; ++x)
                for (int z = 0; z < cellSizeZ; ++z)
                    count += selector(cells[x, z]);
            return count;
        }
    }
}