namespace Core
{
    public class ScriptLoadPath : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.LoadPath;

        public uint PathID { get; set; }
        public uint IsRepeatable { get; set; }
    }
}