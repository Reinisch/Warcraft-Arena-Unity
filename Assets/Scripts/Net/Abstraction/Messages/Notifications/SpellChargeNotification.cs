namespace Net
{
    /// <summary>Server → controlling client: a spell charge began recharging. (Bolt: SpellChargeEvent)</summary>
    public readonly struct SpellChargeNotification : INetMessage
    {
        public int SpellId { get; }
        public int CooldownTime { get; }
        public int ServerFrame { get; }

        public SpellChargeNotification(int spellId, int cooldownTime, int serverFrame)
        {
            SpellId = spellId;
            CooldownTime = cooldownTime;
            ServerFrame = serverFrame;
        }
    }
}
