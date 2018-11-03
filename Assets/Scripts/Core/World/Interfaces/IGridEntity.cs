namespace Core
{
    public interface IGridEntity<TGridEntity> where TGridEntity : class
    {
        GridReference<TGridEntity> GridRef { get; }

        bool IsInGrid();
        void AddToGrid(GridReferenceManager<TGridEntity> refManager);
        void RemoveFromGrid();
    }
}