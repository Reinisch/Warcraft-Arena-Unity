namespace Core
{
    public class ScriptKillCredit : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.KillCredit;

        public uint CreatureEntry { get; set; }
        public uint Flags { get; set; }
    }
}