using Microsoft.Xna.Framework;

namespace BasicRpgEngine.Characters
{
    public interface ICharacter
    {
        Character Character{ get; }
        Color Team { get; } 
    }
}