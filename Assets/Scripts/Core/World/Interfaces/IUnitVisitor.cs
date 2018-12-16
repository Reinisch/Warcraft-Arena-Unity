namespace Core
{
    public interface IUnitVisitor
    {
        void Visit(Player entity);
        void Visit(Creature entity);
    }
}