namespace Core
{
    public abstract class GridState
    {
        public abstract void DoUpdate(Map map, WorldGrid worldGrid, GridInfo gridInfo, int timeDiff);
    }

    public class InvalidState : GridState
    {
        public override void DoUpdate(Map map, WorldGrid worldGrid, GridInfo gridInfo, int timeDiff)
        {
        }
    }

    public class ActiveState : GridState
    {
        private readonly EntityGridStopper visitor = new EntityGridStopper();

        public override void DoUpdate(Map map, WorldGrid worldGrid, GridInfo gridInfo, int timeDiff)
        {
            // only check grid activity every (expiry/10) ms, because it's really useless to do it every cycle
            gridInfo.UpdateTimeTracker(timeDiff);
            if (!gridInfo.Timer.Passed)
                return;

            if (worldGrid.WorldPlayerCount == 0)
            {
                worldGrid.VisitAllGrids(visitor);
                worldGrid.State = GridStateType.Idle;
            }
        }
    }

    public class IdleState : GridState
    {
        public override void DoUpdate(Map map, WorldGrid worldGrid, GridInfo gridInfo, int timeDiff)
        {
            worldGrid.State = GridStateType.Removal;
        }
    }

    public class RemovalState : GridState
    {
        public override void DoUpdate(Map map, WorldGrid worldGrid, GridInfo gridInfo, int timeDiff)
        {
            if (gridInfo.UnloadLock)
                return;

            gridInfo.UpdateTimeTracker(timeDiff);
            if (gridInfo.Timer.Passed && !map.UnloadGrid(worldGrid, false))
            {
            }
        }
    }
}