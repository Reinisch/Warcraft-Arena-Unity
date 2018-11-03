namespace Core
{
    public class ScriptTempSummonCreature : ScriptInfo
    {
        public override ScriptCommands Command => ScriptCommands.TempSummonCreature;

        public uint CreatureEntry { get; set; }
        public uint DespawnDelay { get; set; }

        public float PosX { get; set; }
        public float PosY { get; set; }
        public float PosZ { get; set; }
        public float Orientation { get; set; }
    }
}