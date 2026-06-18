namespace Core
{
    /// <summary>
    /// Frequently-changing, server-authoritative unit state replicated every tick.
    /// Core owns what's replicable; the net adapter only mirrors/serializes this.
    /// </summary>
    // TODO: sent in full every send interval even when unchanged (EntityNetworkView.ServerUpdate); dirty-gate
    // against the last-sent value to cut bandwidth, but keep heartbeating since the channel is unreliable.
    public struct UnitVitals
    {
        public int Health;
        public int MaxHealth;
        public int Power;
        public DeathState DeathState;
        public EmoteType EmoteType;
        public int ModelId;
        public ClassType ClassType;
        public SpellPowerType DisplayPowerType;
        public int ComboPoints;
        public UnitVisualEffectFlags VisualEffects;
    }
}
