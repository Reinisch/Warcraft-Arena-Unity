namespace Core
{
    public struct SpellResourceCost
    {
        public SpellPowerType SpellPower;
        public int Amount;

        public SpellResourceCost(SpellPowerType spellPower, int amount)
        {
            SpellPower = spellPower;
            Amount = amount;
        }
    }
}