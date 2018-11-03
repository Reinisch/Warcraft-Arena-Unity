namespace Core
{
    public class ScriptModel : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.Model;

        public uint ModelID { get; set; }
    }
}