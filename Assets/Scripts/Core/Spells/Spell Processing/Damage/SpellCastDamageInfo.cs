namespace Core
{
    public class SpellCastDamageInfo
    {
        public Unit Target { get; set; }
        public Unit Attacker { get; set; }
        public int SpellId { get; set; }
        public SpellSchoolMask SchoolMask { get; set; }

        public int Damage { get; set; }
        public int Absorb { get; set; }
        public int Resist { get; set; }

        public HitType HitInfo { get; set; }

        public SpellCastDamageInfo(Unit attacker, Unit target, int spellId, SpellSchoolMask schoolMask)
        {
            Target = target;
            Attacker = attacker;
            SpellId = spellId;
            Damage = 0;
            SchoolMask = schoolMask;

            Absorb = 0;
            Resist = 0;
            HitInfo = HitType.NormalSwing;
        }
    }
}