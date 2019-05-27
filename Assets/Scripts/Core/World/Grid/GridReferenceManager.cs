using Common;

namespace Core
{
    public class GridReferenceManager<TGridEntity> : ReferenceManager<TGridEntity, GridReferenceManager<TGridEntity>> where TGridEntity : class
    {
        public new GridReference<TGridEntity> FirstReference => (GridReference<TGridEntity>) base.FirstReference;
    }
}