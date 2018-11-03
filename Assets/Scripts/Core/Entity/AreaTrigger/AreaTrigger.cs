namespace Core
{
    public class AreaTrigger : WorldEntity, IGridEntity<AreaTrigger>
    {
        public GridReference<AreaTrigger> GridRef { get; private set; }


        public bool IsInGrid() { throw new System.NotImplementedException(); }

        public void AddToGrid(GridReferenceManager<AreaTrigger> refManager) { throw new System.NotImplementedException(); }

        public void RemoveFromGrid() { throw new System.NotImplementedException(); }
    }
}