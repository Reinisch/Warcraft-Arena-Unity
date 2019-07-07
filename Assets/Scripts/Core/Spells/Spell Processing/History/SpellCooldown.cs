namespace Core
{
    public class SpellCooldown
    {
        public int Cooldown { get; internal set; }
        public int CooldownLeft { get; internal set; }
        public bool OnHold { get; internal set; }

        public SpellCooldown(int cooldown, int cooldownLeft, bool onHold = false)
        {
            Cooldown = cooldown;
            CooldownLeft = cooldownLeft;
            OnHold = onHold;
        }
    }
}