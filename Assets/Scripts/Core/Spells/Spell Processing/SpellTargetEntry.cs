namespace Core
{
    internal class SpellTargetEntry
    {
        public Unit Target { get; set; }
        public int Delay { get; set; }
        public SpellMissType MissCondition { get; set; }
        public SpellMissType ReflectResult { get; set; }
        public int EffectMask { get; set; }
        public bool Processed { get; set; }
        public bool Alive { get; set; }
        public bool Crit { get; set; }
    }
}