using System;
using Common;
using Core;
using Net;
using UnityEngine;

namespace Server
{
    /// <summary>
    /// Server-authoritative player movement bridge (old project's GamePlayerListener). The owned player's
    /// movement is client-authoritative, so server-side changes the client can't derive — speed rate
    /// (auras like Blazing Speed) and teleports — are sent to the controlling connection, which applies them
    /// to its local player. Only sends when this instance has server logic.
    /// </summary>
    public sealed class ServerPlayerBroadcaster : IDisposable
    {
        private readonly INetworkMessageBus bus;
        private readonly EventBus eventBus;
        private readonly World world;
        private readonly INetConnectionPlayers connectionPlayers;

        public ServerPlayerBroadcaster(INetworkMessageBus bus, EventBus eventBus, World world,
            INetConnectionPlayers connectionPlayers)
        {
            this.bus = bus;
            this.eventBus = eventBus;
            this.world = world;
            this.connectionPlayers = connectionPlayers;

            eventBus.RegisterEvent<Unit, UnitMoveType, float>(GameEvents.ServerPlayerSpeedChanged, OnSpeedChanged);
            eventBus.RegisterEvent<Unit, Vector3>(GameEvents.ServerUnitTeleported, OnTeleported);
        }

        private void OnSpeedChanged(Unit unit, UnitMoveType moveType, float rate)
        {
            if (world.HasServerLogic && connectionPlayers.TryGetConnection(unit, out NetId connection))
                bus.Send(new PlayerSpeedRateChanged(moveType, rate), NetTarget.To(connection));
        }

        private void OnTeleported(Unit unit, Vector3 position)
        {
            if (world.HasServerLogic && connectionPlayers.TryGetConnection(unit, out NetId connection))
                bus.Send(new SpellPlayerTeleport(position), NetTarget.To(connection));
        }

        public void Dispose()
        {
            eventBus.UnregisterEvent<Unit, UnitMoveType, float>(GameEvents.ServerPlayerSpeedChanged, OnSpeedChanged);
            eventBus.UnregisterEvent<Unit, Vector3>(GameEvents.ServerUnitTeleported, OnTeleported);
        }
    }
}
