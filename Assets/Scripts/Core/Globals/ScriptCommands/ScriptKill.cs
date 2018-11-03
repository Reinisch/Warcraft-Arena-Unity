namespace Core
{
    public class ScriptKill : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.Kill;

        public int RemoveCorpse { get; set; }
    }
}