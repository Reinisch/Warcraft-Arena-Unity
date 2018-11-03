namespace Core
{
    public class ScriptTalk  : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.Talk;

        public uint ChatType { get; set; }
        public uint Flags { get; set; }
        public int TextID { get; set; }
    }
}