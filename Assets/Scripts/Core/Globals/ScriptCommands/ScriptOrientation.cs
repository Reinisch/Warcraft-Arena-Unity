namespace Core
{
    public class ScriptOrientation : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.Orientation;

        public uint Flags { get; set; }
        public float Orientation { get; set; }
    }
}