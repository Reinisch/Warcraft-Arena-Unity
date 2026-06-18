namespace Net
{
    public readonly struct ArenaParticipantResult
    {
        public NetId Id { get; }
        public string Name { get; }
        public byte Team { get; }
        public int DamageDone { get; }
        public int HealingDone { get; }

        public ArenaParticipantResult(NetId id, string name, byte team, int damageDone, int healingDone)
        {
            Id = id;
            Name = name;
            Team = team;
            DamageDone = damageDone;
            HealingDone = healingDone;
        }
    }

    public readonly struct ArenaMatchResultNotification : INetMessage
    {
        public byte Result { get; }
        public ArenaParticipantResult[] Participants { get; }

        public ArenaMatchResultNotification(byte result, ArenaParticipantResult[] participants)
        {
            Result = result;
            Participants = participants;
        }
    }
}
