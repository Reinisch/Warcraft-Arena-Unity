namespace Core
{
    public class ScriptMoveTo : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.MoveTo;

        public uint TravelTime { get; set; }

        public float DestX { get; set; }
        public float DestY { get; set; }
        public float DestZ { get; set; }
    }
}