namespace Core
{
    public class Guardian : Minion
    {
        private float[] StatFromOwner { get; set; } 

        public int BonusSpellDamage { get; set; }

        public Guardian(Unit owner, bool isWorldObject) : base(owner, isWorldObject)
        {
            StatFromOwner = new float[StatHelper.MaxStats];
        }

        public override void InitStats(uint duration) { }
        public override void InitSummon() { }

        public override bool UpdateStats(Stats stat) { return true; }
        public override bool UpdateAllStats() { return true; }
        public override void UpdateResistances(SpellSchools school) { }
        public override void UpdateArmor() { }
        public override void UpdateMaxHealth() { }
        public override void UpdateMaxPower(PowerType power) { }
        public override void UpdateAttackPowerAndDamage(bool ranged = false) { }
        public override void UpdateDamagePhysical(WeaponAttackType attType) { }
    }
}