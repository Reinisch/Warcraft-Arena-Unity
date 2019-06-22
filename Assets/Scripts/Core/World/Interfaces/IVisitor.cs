namespace Core
{
    public interface IVisitor : IUnitVisitor
    {
        void Visit<T>(T enttity) where T : Entity;
    }
}