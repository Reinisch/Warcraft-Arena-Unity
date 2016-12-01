using BasicRpgEngine.Characters;

namespace BasicRpgEngine.Characters.AI
{
    public interface ICreatureStrategy
    {
        void ApplyStrategy(Creature monster, World world);
    }
}