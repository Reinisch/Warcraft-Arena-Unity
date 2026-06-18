using System;
using System.Collections.Generic;
using Common;
using Core;
using Net;

namespace Server
{
    /// <summary>
    /// Server → client bridge for arena state. The <see cref="ArenaController"/> raises framework-neutral
    /// <see cref="GameEvents.ServerArenaStateChanged"/> / <see cref="GameEvents.ServerArenaMatchEnded"/>
    /// events; this forwards them to clients as <see cref="ArenaStateNotification"/> /
    /// <see cref="ArenaMatchResultNotification"/> for the HUD. The last state is replayed to late joiners.
    /// </summary>
    public sealed class ServerArenaBroadcaster : IDisposable
    {
        private readonly INetworkMessageBus bus;
        private readonly EventBus eventBus;
        private readonly World world;
        private readonly INetEntityRegistry registry;
        private readonly INetworkController controller;

        private ArenaState lastState;
        private bool hasState;

        public ServerArenaBroadcaster(INetworkMessageBus bus, EventBus eventBus, World world,
            INetEntityRegistry registry, INetworkController controller)
        {
            this.bus = bus;
            this.eventBus = eventBus;
            this.world = world;
            this.registry = registry;
            this.controller = controller;

            eventBus.RegisterEvent<ArenaState>(GameEvents.ServerArenaStateChanged, OnStateChanged);
            eventBus.RegisterEvent<ArenaMatchReport>(GameEvents.ServerArenaMatchEnded, OnMatchEnded);
            controller.PeerConnected += OnPeerConnected;
        }

        private void OnStateChanged(ArenaState state)
        {
            if (!world.HasServerLogic)
                return;

            lastState = state;
            hasState = true;
            bus.Send(ToNotification(state), NetTarget.AllClients);
        }

        private void OnMatchEnded(ArenaMatchReport report)
        {
            if (!world.HasServerLogic)
                return;

            IReadOnlyList<ArenaParticipantStats> stats = report.Participants;
            var participants = new ArenaParticipantResult[stats.Count];
            for (int i = 0; i < stats.Count; i++)
            {
                ArenaParticipantStats p = stats[i];
                participants[i] = new ArenaParticipantResult(
                    registry.GetId(p.Player),
                    p.Player != null ? p.Player.Name : string.Empty,
                    (byte)(p.TeamA ? 0 : 1),
                    p.DamageDone, p.HealingDone);
            }

            bus.Send(new ArenaMatchResultNotification((byte)report.Result, participants), NetTarget.AllClients);
        }

        private void OnPeerConnected(NetId peer)
        {
            if (world.HasServerLogic && hasState)
                bus.Send(ToNotification(lastState), NetTarget.To(peer));
        }

        private static ArenaStateNotification ToNotification(ArenaState state) =>
            new ArenaStateNotification(
                (byte)state.Phase, state.Countdown,
                (byte)state.AliveTeamA, (byte)state.TotalTeamA,
                (byte)state.AliveTeamB, (byte)state.TotalTeamB);

        public void Dispose()
        {
            eventBus.UnregisterEvent<ArenaState>(GameEvents.ServerArenaStateChanged, OnStateChanged);
            eventBus.UnregisterEvent<ArenaMatchReport>(GameEvents.ServerArenaMatchEnded, OnMatchEnded);
            controller.PeerConnected -= OnPeerConnected;
        }
    }
}
