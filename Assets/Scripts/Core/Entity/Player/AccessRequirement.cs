namespace Core
{
    public class AccessRequirement
    {
        public byte levelMin { get; set; }
        public byte levelMax { get; set; }
        public uint item { get; set; }
        public uint item2 { get; set; }
        public uint quest_A { get; set; }
        public uint quest_H { get; set; }
        public uint achievement { get; set; }
        public string questFailedText { get; set; }
    }
}