namespace Core
{
    public interface IVisitor : IUnitVisitor
    {
        void Visit(Entity entity);

        void Visit<T>(T enttiy) where T : Entity;
    }
}