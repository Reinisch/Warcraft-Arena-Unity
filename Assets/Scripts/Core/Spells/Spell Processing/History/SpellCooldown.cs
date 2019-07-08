namespace Core
{
    public class SpellCooldown
    {
        public int SpellId { get; }
        public int Cooldown { get; internal set; }
        public int CooldownLeft { get; internal set; }
        public bool OnHold { get; internal set; }

        public SpellCooldown(int cooldown, int cooldownLeft, int spellId, bool onHold = false)
        {
            Cooldown = cooldown;
            CooldownLeft = cooldownLeft;
            SpellId = spellId;
            OnHold = onHold;
        }
    }
}