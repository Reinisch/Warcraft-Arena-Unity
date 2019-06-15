namespace Core
{
    public interface IVisitor : IUnitVisitor
    {
        void Visit(Entity entity);

        void Visit<T>(T enttity) where T : Entity;
    }
}