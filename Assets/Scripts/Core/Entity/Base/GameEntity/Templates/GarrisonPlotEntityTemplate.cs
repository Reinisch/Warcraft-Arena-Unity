namespace Core
{
    public class GarrisonPlotEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.GarrisonPlot;

        public uint PlotInstance { get; set; }                            // 0 Plot Instance, References: GarrPlotInstance, NoValue = 0
        public int SpawnMap { get; set; }                                 // 1 Spawn Map, References: Map, NoValue = -1
    }
}