namespace Core
{
    public class UiLinkEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.UiLink;

        public uint UILinkType { get; set; }                              // 0 UI Link Type, enum { Adventure Journal, Obliterum Forge, }; Default: Adventure Journal
        public uint AllowMounted { get; set; }                            // 1 allowMounted, enum { false, true, }; Default: false
        public uint GiganticAoi { get; set; }                             // 2 Gigantic AOI, enum { false, true, }; Default: false
        public uint SpellFocusType { get; set; }                          // 3 spellFocusType, References: SpellFocusObject, NoValue = 0
        public uint Radius { get; set; }                                  // 4 radius, int, Min value: 0, Max value: 50, Default value: 10
    }
}