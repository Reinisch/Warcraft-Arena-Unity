namespace Core
{
    public class SpellNonMeleeDamage
    {
        public Unit Target { get; set; }
        public Unit Attacker { get; set; }
        public ulong CastId { get; set; }
        public int SpellId { get; set; }
        public SpellSchoolMask SchoolMask { get; set; }

        public int Damage { get; set; }
        public int Absorb { get; set; }
        public int Resist { get; set; }
        public int Blocked { get; set; }
        public int CleanDamage { get; set; }

        public HitInfo HitInfo { get; set; }
        public bool PeriodicLog { get; set; }
        public long PreHitHealth { get; set; }


        public SpellNonMeleeDamage(Unit attacker, Unit target, int spellId, SpellSchoolMask schoolMask, ulong castId = 0)
        {
            Target = target;
            Attacker = attacker;
            CastId = castId;
            SpellId = spellId;
            Damage = 0;
            SchoolMask = schoolMask;

            Absorb = 0;
            Resist = 0;
            Blocked = 0;
            CleanDamage = 0;
            PeriodicLog = false;
            HitInfo = HitInfo.Normalswing;
            PreHitHealth = target.GetHealth();
        }
    }
}