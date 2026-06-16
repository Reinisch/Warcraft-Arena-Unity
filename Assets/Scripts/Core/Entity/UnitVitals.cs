namespace Core
{
    /// <summary>
    /// Frequently-changing, server-authoritative unit state replicated every tick (health/power/death/emote).
    /// Max health/power are NOT here — they derive from the unit's definition on the client at spawn. Core
    /// owns what's replicable; the net adapter only mirrors/serializes this.
    /// </summary>
    public struct UnitVitals
    {
        public int Health;
        public int Power;
        public DeathState DeathState;
        public EmoteType EmoteType;
        // Current display model — changes at runtime via transform auras (Polymorph → sheep), so it must
        // replicate continuously, not just at spawn.
        public int ModelId;
        // Current class — players switch class at runtime (SwitchClass), so it replicates continuously too;
        // the client applies it to rebuild its action bar / available powers.
        public ClassType ClassType;
        // Which power the unit currently displays (changes at runtime, e.g. Cat Form → Energy) + combo points
        // (gained/consumed by abilities). The bar's MAX derives from the class power definition on the client.
        public SpellPowerType DisplayPowerType;
        public int ComboPoints;
        // Visual-effect flags (stealth/invisibility transparency) — toggled at runtime by auras (Prowl, Greater
        // Invisibility), so they replicate continuously; the client renderer fades the model on change.
        public UnitVisualEffectFlags VisualEffects;
    }
}
