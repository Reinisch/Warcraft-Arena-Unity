namespace BasicRpgEngine.Spells
{
    public class SpellModificationInformation
    {
        public string SpellName { get; set; }
        public DamageType DamageType { get; set; }
        public float PhysicalDamageMulMod { get; set; }
        public float FrostDamageMulMod { get; set; }
        public float FireDamageMulMod { get; set; }
        public float ArcaneDamageMulMod { get; set; }
        public float NatureDamageMulMod { get; set; }
        public float ShadowDamageMulMod { get; set; }
        public float HolyDamageMulMod { get; set; }
        public float CriticalChanceAddMod { get; set; }
        public float CriticalChanceMulMod { get; set; }
        public float CriticalDamageMultiplierAddMod { get; set; }
        public float HasteRatingAddMod { get; set; }
        public float DamageMultiplier { get; set; }
        public float AdditionalDamage { get; set; }
        public bool InstantCast { get; set; }
        public bool IsCrit { get; set; }
        public bool IsCastFailed { get; set; }

        public SpellModificationInformation(string name)
        {
            SpellName = name;
            CriticalChanceAddMod = 0;
            CriticalChanceMulMod = 0;
            CriticalDamageMultiplierAddMod = 0;
            HasteRatingAddMod = 0;
            DamageMultiplier = 0;
            AdditionalDamage = 0;
            InstantCast = false;
            IsCrit = false;
            IsCastFailed = false;
        }
    }
}