namespace Core
{
    /// <summary>
    /// Compact, display-only view of one visible aura slot for replication. Mirrors what the client's aura
    /// visuals read (id + duration + charges); the client ticks DurationLeft down locally between updates.
    /// </summary>
    public struct NetAuraSlot
    {
        public int AuraId;
        public int DurationMax;
        public int DurationLeft;
        public int Charges;

        public bool HasAura => AuraId > 0;
    }
}
