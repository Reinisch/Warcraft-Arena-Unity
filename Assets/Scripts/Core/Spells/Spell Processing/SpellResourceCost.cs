namespace Core
{
    public struct SpellResourceCost
    {
        public SpellResourceType SpellResource;
        public int Amount;

        public SpellResourceCost(SpellResourceType spellResource, int amount)
        {
            SpellResource = spellResource;
            Amount = amount;
        }
    }
}