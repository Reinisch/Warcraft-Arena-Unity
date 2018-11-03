namespace Core
{
    public class PhasableMoEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.PhaseableMo;

        public int SpawnMap { get; set; }                                 // 0 Spawn Map, References: Map, NoValue = -1
        public uint AreaNameSet { get; set; }                             // 1 Area Name Set (Index), int, Min value: -2147483648, Max value: 2147483647, Default value: 0
        public uint DoodadSetA { get; set; }                              // 2 Doodad Set A, int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint DoodadSetB { get; set; }                              // 3 Doodad Set B, int, Min value: 0, Max value: 2147483647, Default value: 0
    }
}