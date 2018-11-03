namespace Core
{
    public class ScriptCallScriptToUnit : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.CallScriptToUnit;

        public uint CreatureEntry { get; set; }
        public uint ScriptID { get; set; }
        public uint ScriptType { get; set; }
    }
}