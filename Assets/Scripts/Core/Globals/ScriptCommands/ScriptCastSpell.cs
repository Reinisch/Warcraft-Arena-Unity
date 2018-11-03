namespace Core
{
    public class ScriptCastSpell : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.CastSpell;

        public uint SpellID { get; set; }
        public uint Flags { get; set; }
        public int CreatureEntry { get; set; }

        public float SearchRadius { get; set; }
    }
}