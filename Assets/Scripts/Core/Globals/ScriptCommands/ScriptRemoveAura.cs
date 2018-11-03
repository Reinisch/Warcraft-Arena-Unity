namespace Core
{
    public class ScriptRemoveAura : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.RemoveAura;

        public uint SpellID { get; set; }
        public uint Flags { get; set; }
    }
}