namespace Core
{
    public class TextEntityTemplate : GameEntityTemplate
    {
        public override GameEntityTypes Type => GameEntityTypes.Text;

        public uint PageID { get; set; }                    // 0 pageID, References: PageText, NoValue = 0
        public uint Language { get; set; }                  // 1 language, References: Languages, NoValue = 0
        public uint PageMaterial { get; set; }              // 2 pageMaterial, References: PageTextMaterial, NoValue = 0
        public uint AllowMounted { get; set; }              // 3 allowMounted, enum { false, true, }; Default: false
        public uint ConditionID1 { get; set; }              // 4 conditionID1, References: PlayerCondition, NoValue = 0
        public uint NeverUsableWhileMounted { get; set; }   // 5 Never Usable While Mounted, enum { false, true, }; Default: false

        public override bool IsUsableMounted => AllowMounted != 0;
    }
}