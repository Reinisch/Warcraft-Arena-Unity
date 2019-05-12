namespace Core
{
    public interface IWorldEntityGridVisitor
    {
        void Visit<TEntity>(GridReferenceManager<TEntity> container) where TEntity : WorldEntity;
    }
}