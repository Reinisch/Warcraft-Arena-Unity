using System;
using Core;
using Net;

namespace Server
{
    /// <summary>
    /// Server-authoritative command handling. Subscribes to client request messages and applies them to
    /// the world — the gameplay calls that used to live inline in InputReference. In single-player / host
    /// this runs in-process; on a dedicated server it's the only place these run.
    /// </summary>
    public sealed class ServerCommandRouter : IDisposable
    {
        private readonly INetworkMessageBus bus;
        private readonly World world;
        private readonly BalanceReference balance;
        private readonly INetEntityRegistry registry;
        private readonly INetConnectionPlayers connectionPlayers;
        private readonly IDisposable[] subscriptions;

        public ServerCommandRouter(INetworkMessageBus bus, World world, BalanceReference balance,
            INetEntityRegistry registry, INetConnectionPlayers connectionPlayers)
        {
            this.bus = bus;
            this.world = world;
            this.balance = balance;
            this.registry = registry;
            this.connectionPlayers = connectionPlayers;

            subscriptions = new[]
            {
                bus.Subscribe<SpellCastRequest>(OnSpellCast),
                bus.Subscribe<SpellCastDestinationRequest>(OnSpellCastDestination),
                bus.Subscribe<SpellCastTargetingRequest>(OnSpellCastTargeting),
                bus.Subscribe<SpellCastCancelRequest>(OnSpellCastCancel),
                bus.Subscribe<PlayerEmoteRequest>(OnEmote),
                bus.Subscribe<PlayerClassChangeRequest>(OnClassChange),
                bus.Subscribe<TargetSelectionRequest>(OnTargetSelection),
                bus.Subscribe<PlayerChatRequest>(OnChat),
            };
        }

        // Resolve the caster from the sending connection so each client's input drives ITS own player.
        // Falls back to the local player (single-player / before the sender's player is registered).
        private Player ResolveCaster(NetContext ctx)
        {
            if (connectionPlayers.TryGetPlayer(ctx.Sender, out Player player) && player != null)
                return player;

            return world.PlayerManager.Player;
        }

        private void OnSpellCast(SpellCastRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster == null || !caster.ExistsIn(world))
                return;

            if (!balance.SpellInfosById.TryGetValue(msg.SpellId, out SpellInfo spellInfo))
            {
                Reply(msg.SpellId, SpellCastResult.SpellUnavailable, ctx);
                return;
            }

            SpellCastResult result = caster.Spells.CastSpell(spellInfo, new SpellCastingOptions(movementFlags: msg.MovementFlags));
            Reply(msg.SpellId, result, ctx);
        }

        private void OnSpellCastDestination(SpellCastDestinationRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster == null || !caster.ExistsIn(world))
                return;

            if (!balance.SpellInfosById.TryGetValue(msg.SpellId, out SpellInfo spellInfo))
            {
                Reply(msg.SpellId, SpellCastResult.SpellUnavailable, ctx);
                return;
            }

            SpellCastResult result = caster.Spells.CastSpell(spellInfo,
                new SpellCastingOptions(new SpellExplicitTargets { Destination = msg.Destination }));
            Reply(msg.SpellId, result, ctx);
        }

        private void OnSpellCastTargeting(SpellCastTargetingRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster == null || !caster.ExistsIn(world))
                return;

            if (!balance.SpellInfosById.TryGetValue(msg.SpellId, out SpellInfo spellInfo))
            {
                Reply(msg.SpellId, SpellCastResult.SpellUnavailable, ctx);
                return;
            }

            SpellCastResult result = caster.Spells.CastSpell(spellInfo,
                new SpellCastingOptions(targetingSource: msg.TargetingSource, targetingRotation: msg.TargetingRotation));
            Reply(msg.SpellId, result, ctx);
        }

        private void OnSpellCastCancel(SpellCastCancelRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster != null && caster.SpellCast.IsCasting)
                caster.SpellCast.Cancel();
        }

        private void OnEmote(PlayerEmoteRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster != null && caster.ExistsIn(world))
                caster.ModifyEmoteState(msg.EmoteType);
        }

        private void OnClassChange(PlayerClassChangeRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster != null && caster.ExistsIn(world))
                caster.SwitchClass(msg.ClassType);
        }

        private void OnTargetSelection(TargetSelectionRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster == null || !caster.ExistsIn(world))
                return;

            registry.TryGet(msg.TargetId, out Unit target);
            caster.SetTarget(target);
        }

        private void OnChat(PlayerChatRequest msg, NetContext ctx)
        {
            Player caster = ResolveCaster(ctx);
            if (caster == null || !caster.ExistsIn(world))
                return;

            bus.Send(new UnitChatMessage(registry.GetId(caster), caster.Name, msg.Message), NetTarget.Everyone);
        }

        private void Reply(int spellId, SpellCastResult result, NetContext ctx)
        {
            bus.Send(new SpellCastResultNotification(spellId, result), NetTarget.To(ctx.Sender));
        }

        public void Dispose()
        {
            for (int i = 0; i < subscriptions.Length; i++)
                subscriptions[i]?.Dispose();
        }
    }
}
