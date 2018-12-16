namespace Core
{
    public interface IEntityGridVisitor
    {
        void Visit<TEntity>(GridReferenceManager<TEntity> container) where TEntity : Entity;
    }

    public interface IWorldGridVisitor : IEntityGridVisitor
    {
    
    }

    public interface IGridGridVisitor : IEntityGridVisitor
    {

    }
}