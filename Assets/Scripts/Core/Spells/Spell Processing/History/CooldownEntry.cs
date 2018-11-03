namespace Core
{
    public class CooldownEntry
    {
        public int SpellId { get; set; }
        public float CooldownEnd { get; set; }
        public bool OnHold { get; set; }

        public CooldownEntry(int spellId, float cooldownEnd, bool onHold = false)
        {
            SpellId = spellId;
            CooldownEnd = cooldownEnd;
            OnHold = onHold;
        }
    }
}