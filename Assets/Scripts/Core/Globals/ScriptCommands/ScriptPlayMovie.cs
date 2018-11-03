namespace Core
{
    public class ScriptPlayMovie : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.PlayMovie;

        public uint MovieID { get; set; }
    }
}