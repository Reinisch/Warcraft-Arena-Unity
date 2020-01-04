namespace Core
{
    public class SpellChargeCooldown
    {
        public int SpellId { get; }
        public int ChargeTime { get; internal set; }
        public int ChargeTimeLeft { get; internal set; }

        public SpellChargeCooldown(int chargeTime, int chargeTimeLeft, int spellId)
        {
            ChargeTime = chargeTime;
            ChargeTimeLeft = chargeTimeLeft;
            SpellId = spellId;
        }
    }
}