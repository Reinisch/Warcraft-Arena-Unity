namespace Core
{
    public class SpellChargeCategoryEntry
    {
        public uint Id { get; set; }
        public float ChargeRecoveryTime { get; set; }
        public int MaxCharges { get; set; }
        public int ChargeCategoryType { get; set; }
    }
}