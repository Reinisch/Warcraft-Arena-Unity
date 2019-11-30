namespace Core
{
    public interface IUnitVisitor
    {
        void Visit(Player player);
        void Visit(Creature creature);
    }
}