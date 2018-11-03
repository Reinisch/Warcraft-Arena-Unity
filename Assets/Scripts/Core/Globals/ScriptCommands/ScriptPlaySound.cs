namespace Core
{
    public class ScriptPlaySound : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.PlaySound;

        public uint SoundID { get; set; }
        public uint Flags { get; set; }
    }
}