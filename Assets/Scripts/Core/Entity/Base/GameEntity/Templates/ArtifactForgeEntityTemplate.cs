namespace Core
{
    public class ArtifactForgeEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.ArtifactForge;

        public uint ConditionID1 { get; set; }                          // 0 conditionID1, References: PlayerCondition, NoValue = 0
        public uint LargeAoi { get; set; }                              // 1 Large AOI, enum { false, true, }; Default: false
        public uint IgnoreBoundingBox { get; set; }                     // 2 Ignore Bounding Box, enum { false, true, }; Default: false
        public uint CameraMode { get; set; }                            // 3 Camera Mode, References: CameraMode, NoValue = 0
        public uint FadeRegionRadius { get; set; }                      // 4 Fade Region Radius, int, Min value: 0, Max value: 2147483647, Default value: 0
    }
}