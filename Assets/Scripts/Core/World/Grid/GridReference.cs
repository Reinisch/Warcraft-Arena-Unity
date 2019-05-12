namespace Core
{
    public class GridReference<TGridEntity> : Reference<TGridEntity, GridReferenceManager<TGridEntity>> where TGridEntity : class
    {
        public new GridReference<TGridEntity> Next => (GridReference<TGridEntity>)base.Next;
    }
}