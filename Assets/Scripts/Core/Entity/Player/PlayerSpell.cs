namespace Core
{
    public class PlayerSpell
    {
        public PlayerSpellState State;
        public bool Active;                     // show in spellbook
        public bool Dependent;                  // learned as result another spell learn, skill grow, quest reward, etc
        public bool Disabled;                   // first rank has been learned in result talent learn but currently talent unlearned, save max learned ranks
    }
}