namespace Net
{
    /// <summary>
    /// Server → client: a unit was healed. Mirrors the Core <c>GameEvents.SpellHealingDone</c>
    /// (healer, target, amount, crit) so a remote client can re-raise it for healing feedback.
    /// </summary>
    public readonly struct SpellHealingDone : INetMessage
    {
        public NetId HealerId { get; }
        public NetId TargetId { get; }
        public int Heal { get; }
        public bool IsCrit { get; }

        public SpellHealingDone(NetId healerId, NetId targetId, int heal, bool isCrit)
        {
            HealerId = healerId;
            TargetId = targetId;
            Heal = heal;
            IsCrit = isCrit;
        }
    }
}
