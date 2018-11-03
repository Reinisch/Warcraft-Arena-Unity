namespace Core
{
    public class ScriptTeleportTo : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.TeleportTo;

        public uint MapID { get; set; }
        public uint Flags { get; set; }

        public float DestX { get; set; }
        public float DestY { get; set; }
        public float DestZ { get; set; }
        public float Orientation { get; set; }
    }
}