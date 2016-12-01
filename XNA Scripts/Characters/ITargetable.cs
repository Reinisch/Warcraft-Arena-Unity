using BasicRpgEngine.Physics;

namespace BasicRpgEngine.Characters
{
    public interface ITargetable : ICharacter, IMoveble
    {
        int ID { get; }
        bool NeedsDispose { get; }
        bool IsHumanPlayer { get; }
        bool IsDead { get; }
        GameMap GameMapRef { get; }
    }
}