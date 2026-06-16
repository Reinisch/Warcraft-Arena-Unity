using System;
using Core;
using Net;

namespace Client
{
    /// <summary>
    /// CLIENT-SIDE player movement bridge (old project's NetworkPlayerListener). Applies server-sent speed
    /// rate + teleport to the local player — movement is client-authoritative, so these server changes are
    /// pushed in. Active only on a pure remote client (a host applies them directly in-process).
    /// </summary>
    public sealed class ClientPlayerHandler : IDisposable
    {
        private readonly World world;
        private readonly IDisposable[] subscriptions;

        public ClientPlayerHandler(INetworkMessageBus bus, World world)
        {
            this.world = world;

            subscriptions = new[]
            {
                bus.Subscribe<PlayerSpeedRateChanged>(OnSpeedChanged),
                bus.Subscribe<SpellPlayerTeleport>(OnTeleport),
            };
        }

        private bool IsRemoteClient => world.HasClientLogic && !world.HasServerLogic;

        private void OnSpeedChanged(PlayerSpeedRateChanged msg, NetContext ctx)
        {
            if (IsRemoteClient && world.PlayerManager.Player != null)
                world.PlayerManager.Player.ApplyNetworkSpeed(msg.MoveType, msg.SpeedRate);
        }

        private void OnTeleport(SpellPlayerTeleport msg, NetContext ctx)
        {
            if (IsRemoteClient && world.PlayerManager.Player != null)
                world.PlayerManager.Player.ApplyNetworkTeleport(msg.TargetPosition);
        }

        public void Dispose()
        {
            for (int i = 0; i < subscriptions.Length; i++)
                subscriptions[i]?.Dispose();
        }
    }
}
