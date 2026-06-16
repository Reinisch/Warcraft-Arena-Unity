namespace Net
{
    /// <summary>
    /// Server → client: a unit was hit by a spell. Mirrors the Core <c>GameEvents.SpellHit</c>
    /// (target, spellId) so a remote client can re-raise it for hit visuals/sound. Routed to the
    /// target's observers via <see cref="NetTarget.Observers"/>.
    /// </summary>
    public readonly struct UnitSpellHit : INetMessage
    {
        public NetId TargetId { get; }
        public int SpellId { get; }

        public UnitSpellHit(NetId targetId, int spellId)
        {
            TargetId = targetId;
            SpellId = spellId;
        }
    }
}
