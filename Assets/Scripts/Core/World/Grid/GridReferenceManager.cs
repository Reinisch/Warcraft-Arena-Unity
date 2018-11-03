namespace Core
{
    public class GridReferenceManager<TGridEntity> : ReferenceManager<GridReferenceManager<TGridEntity>, TGridEntity> where TGridEntity : class
    {
        public new GridReference<TGridEntity> FirstReference => (GridReference<TGridEntity>) base.FirstReference;
        public new GridReference<TGridEntity> LastReference => (GridReference<TGridEntity>) base.LastReference;
    }
}