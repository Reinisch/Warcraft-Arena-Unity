namespace Core
{
    public class SpellTargetInfo
    {
        public ulong TargetId { get; set; }
        public float Delay { get; set; }
        public SpellMissType MissCondition { get; set; }
        public SpellMissType ReflectResult { get; set; }
        public int EffectMask { get; set; }
        public bool Processed { get; set; }
        public bool Alive { get; set; }
        public bool Crit { get; set; }
        public bool ScaleAura { get; set; }
        public int Damage { get; set; }
    }
}