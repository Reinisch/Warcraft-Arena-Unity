namespace Core
{
    public interface IEntityVisitor
    {
        void Visit<TEntity>(GridReferenceManager<TEntity> container) where TEntity : Entity;
    }

    public interface IWorldVisitor : IEntityVisitor
    {
    
    }

    public interface IGridVisitor : IEntityVisitor
    {

    }
}