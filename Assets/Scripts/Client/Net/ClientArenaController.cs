using System;
using System.Collections.Generic;
using Core;
using Net;

namespace Client
{
    public enum ArenaLocalOutcome
    {
        None,
        Won,
        Lost,
        Draw
    }

    public readonly struct ArenaParticipantInfo
    {
        public string Name { get; }
        public bool TeamA { get; }
        public int DamageDone { get; }
        public int HealingDone { get; }

        /// <summary>True for the row belonging to the player viewing this client.</summary>
        public bool IsLocalPlayer { get; }

        public ArenaParticipantInfo(string name, bool teamA, int damageDone, int healingDone, bool isLocalPlayer)
        {
            Name = name;
            TeamA = teamA;
            DamageDone = damageDone;
            HealingDone = healingDone;
            IsLocalPlayer = isLocalPlayer;
        }
    }

    /// <summary>
    /// Client-side, receives the server's <see cref="ArenaStateNotification"/> and exposes arena state.
    /// </summary>
    public sealed class ClientArenaController : IDisposable
    {
        private readonly World world;
        private readonly INetEntityRegistry registry;
        private readonly IDisposable stateSubscription;
        private readonly IDisposable resultSubscription;

        public ArenaPhase Phase { get; private set; }
        public float Countdown { get; private set; }
        public int AliveTeamA { get; private set; }
        public int TotalTeamA { get; private set; }
        public int AliveTeamB { get; private set; }
        public int TotalTeamB { get; private set; }

        public ArenaMatchResult Result { get; private set; }
        public ArenaLocalOutcome LocalOutcome { get; private set; }

        public IReadOnlyList<ArenaParticipantInfo> Participants { get; private set; } = new ArenaParticipantInfo[0];

        public event Action StateChanged;
        public event Action<ArenaMatchResult> MatchEnded;

        public ClientArenaController(INetworkMessageBus bus, World world, INetEntityRegistry registry)
        {
            this.world = world;
            this.registry = registry;
            stateSubscription = bus.Subscribe<ArenaStateNotification>(OnState);
            resultSubscription = bus.Subscribe<ArenaMatchResultNotification>(OnResult);
        }

        public void Dispose()
        {
            stateSubscription?.Dispose();
            resultSubscription?.Dispose();
        }

        private void OnState(ArenaStateNotification msg, NetContext ctx)
        {
            Phase = (ArenaPhase)msg.Phase;
            Countdown = msg.Countdown;
            AliveTeamA = msg.AliveTeamA;
            TotalTeamA = msg.TotalTeamA;
            AliveTeamB = msg.AliveTeamB;
            TotalTeamB = msg.TotalTeamB;

            StateChanged?.Invoke();
        }

        private void OnResult(ArenaMatchResultNotification msg, NetContext ctx)
        {
            Result = (ArenaMatchResult)msg.Result;
            BuildScoreboard(msg);
            MatchEnded?.Invoke(Result);
        }

        private void BuildScoreboard(ArenaMatchResultNotification msg)
        {
            NetId localId = NetId.None;
            Player local = world != null && world.PlayerManager != null ? world.PlayerManager.Player : null;
            if (local != null)
                localId = registry.GetId(local);

            ArenaParticipantResult[] wire = msg.Participants ?? new ArenaParticipantResult[0];
            var rows = new ArenaParticipantInfo[wire.Length];
            bool? localTeamA = null;

            for (int i = 0; i < wire.Length; i++)
            {
                ArenaParticipantResult p = wire[i];
                bool isLocal = localId != NetId.None && p.Id == localId;
                if (isLocal)
                    localTeamA = p.Team == 0;

                rows[i] = new ArenaParticipantInfo(p.Name, p.Team == 0, p.DamageDone, p.HealingDone, isLocal);
            }

            Participants = rows;
            LocalOutcome = ResolveOutcomeAsClient(Result, localTeamA);

            ArenaLocalOutcome ResolveOutcomeAsClient(ArenaMatchResult result, bool? clientsTeamA)
            {
                if (result == ArenaMatchResult.Draw)
                    return ArenaLocalOutcome.Draw;
                if (clientsTeamA == null)
                    return ArenaLocalOutcome.None; // viewer isn't a participant (e.g. a spectator)

                bool won = (result == ArenaMatchResult.TeamA && clientsTeamA.Value) ||
                        (result == ArenaMatchResult.TeamB && !clientsTeamA.Value);
                return won ? ArenaLocalOutcome.Won : ArenaLocalOutcome.Lost;
            }
        }
    }
}
