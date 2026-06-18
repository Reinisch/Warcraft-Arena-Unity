using System.Collections.Generic;

namespace Core
{
    public enum ArenaPhase
    {
        Warmup,
        InProgress,
        Ended
    }

    public enum ArenaMatchResult
    {
        Undecided,
        TeamA,
        TeamB,
        Draw
    }

    public readonly struct ArenaState
    {
        public ArenaPhase Phase { get; }
        public float Countdown { get; }
        public int AliveTeamA { get; }
        public int TotalTeamA { get; }
        public int AliveTeamB { get; }
        public int TotalTeamB { get; }

        public ArenaState(ArenaPhase phase, float countdown, int aliveTeamA, int totalTeamA, int aliveTeamB, int totalTeamB)
        {
            Phase = phase;
            Countdown = countdown;
            AliveTeamA = aliveTeamA;
            TotalTeamA = totalTeamA;
            AliveTeamB = aliveTeamB;
            TotalTeamB = totalTeamB;
        }
    }

    public readonly struct ArenaParticipantStats
    {
        public Player Player { get; }
        public bool TeamA { get; }
        public int DamageDone { get; }
        public int HealingDone { get; }

        public ArenaParticipantStats(Player player, bool teamA, int damageDone, int healingDone)
        {
            Player = player;
            TeamA = teamA;
            DamageDone = damageDone;
            HealingDone = healingDone;
        }
    }

    public sealed class ArenaMatchReport
    {
        public ArenaMatchResult Result { get; }
        public IReadOnlyList<ArenaParticipantStats> Participants { get; }

        public ArenaMatchReport(ArenaMatchResult result, IReadOnlyList<ArenaParticipantStats> participants)
        {
            Result = result;
            Participants = participants;
        }
    }
}
