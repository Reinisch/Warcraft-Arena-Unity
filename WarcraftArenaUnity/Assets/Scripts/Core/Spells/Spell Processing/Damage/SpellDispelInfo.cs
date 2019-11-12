namespace Core
{
    public class SpellDispelInfo
    {
        public Unit Dispeller { get; private set; }
        public uint DispellerSpellId { get; private set; }
        public byte RemovedCharges { get; set; }

        public SpellDispelInfo(Unit dispeller, uint dispellerSpellId, byte chargesRemoved)
        {
            Dispeller = dispeller;
            DispellerSpellId = dispellerSpellId;
            RemovedCharges = chargesRemoved;
        }
    }
}