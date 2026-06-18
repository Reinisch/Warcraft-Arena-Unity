namespace Net
{
    public readonly struct ArenaStateNotification : INetMessage
    {
        public byte Phase { get; }
        public float Countdown { get; }
        public byte AliveTeamA { get; }
        public byte TotalTeamA { get; }
        public byte AliveTeamB { get; }
        public byte TotalTeamB { get; }

        public ArenaStateNotification(byte phase, float countdown,
            byte aliveTeamA, byte totalTeamA, byte aliveTeamB, byte totalTeamB)
        {
            Phase = phase;
            Countdown = countdown;
            AliveTeamA = aliveTeamA;
            TotalTeamA = totalTeamA;
            AliveTeamB = aliveTeamB;
            TotalTeamB = totalTeamB;
        }
    }
}
