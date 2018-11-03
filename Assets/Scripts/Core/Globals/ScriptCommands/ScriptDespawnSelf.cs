namespace Core
{
    public class ScriptDespawnSelf : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.DespawnSelf;

        public uint DespawnDelay { get; set; }
    }
}