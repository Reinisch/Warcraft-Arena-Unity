namespace Core
{
    public class ScriptPlayAnimKit : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.PlayAnimKit;

        public uint AnimKitID { get; set; }
    }
}