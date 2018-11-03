namespace Core
{
    public class TrapdoorEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Trapdoor;

        public uint AutoLink { get; set; }                  // 0 Auto Link, enum { false, true, }; Default: false
        public uint StartOpen { get; set; }                 // 1 startOpen, enum { false, true, }; Default: false
        public uint AutoClose { get; set; }                 // 2 autoClose (ms), int, Min value: 0, Max value: 2147483647, Default value: 0
        public uint BlocksPathsDown { get; set; }           // 3 Blocks Paths Down, enum { false, true, }; Default: false
        public uint PathBlockerBump { get; set; }           // 4 Path Blocker Bump (ft), int, Min value: -2147483648, Max value: 2147483647, Default value: 0

        public override uint AutoCloseTime => AutoClose / TimeHelper.InMilliseconds;
    }
}