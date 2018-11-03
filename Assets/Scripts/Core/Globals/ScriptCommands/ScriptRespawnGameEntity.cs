namespace Core
{
    public class ScriptRespawnGameEntity : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.RespawnGameEntity;

        public uint GOGuid { get; set; }
        public uint DespawnDelay { get; set; }
    }
}