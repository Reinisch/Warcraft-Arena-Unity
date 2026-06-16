namespace Net
{
    /// <summary>
    /// Server → controlling client: a spell entered cooldown. ServerFrame stamps when it started so the
    /// client can compute remaining time against <see cref="INetworkTime.ServerFrame"/>. (Bolt: SpellCooldownEvent)
    /// </summary>
    public readonly struct SpellCooldownNotification : INetMessage
    {
        public int SpellId { get; }
        public int CooldownTime { get; }
        public int ServerFrame { get; }

        public SpellCooldownNotification(int spellId, int cooldownTime, int serverFrame)
        {
            SpellId = spellId;
            CooldownTime = cooldownTime;
            ServerFrame = serverFrame;
        }
    }
}
