using System;
using Common;
using Core;
using Net;

namespace Client
{
    /// <summary>
    /// CLIENT-SIDE result handling. Receives the server's cast result and raises the existing
    /// <see cref="GameEvents.ClientSpellFailed"/> game event on failure — preserving the feedback
    /// <see cref="InputReference"/> used to fire inline. (Bolt equivalent: the client's
    /// OnEvent(SpellCastRequestAnswerEvent).)
    /// </summary>
    public sealed class ClientSpellResultHandler : IDisposable
    {
        private readonly EventBus eventBus;
        private readonly World world;
        private readonly BalanceReference balance;
        private readonly IDisposable subscription;

        public ClientSpellResultHandler(INetworkMessageBus bus, EventBus eventBus, World world, BalanceReference balance)
        {
            this.eventBus = eventBus;
            this.world = world;
            this.balance = balance;
            subscription = bus.Subscribe<SpellCastResultNotification>(OnSpellCastResult);
        }

        // A host's real cast already started its cooldowns; only a pure remote client needs the display copy.
        private bool IsRemoteClient => world.HasClientLogic && !world.HasServerLogic;

        private void OnSpellCastResult(SpellCastResultNotification msg, NetContext ctx)
        {
            if (msg.Result != SpellCastResult.Success)
            {
                eventBus.ExecuteEvent(GameEvents.ClientSpellFailed, msg.Result);
                return;
            }

            // Cast accepted by the server → mirror its cooldown/GCD/charges on our local player for the
            // action bar (server cooldowns aren't replicated; they're deterministic from SpellInfo).
            if (IsRemoteClient && world.PlayerManager.Player != null &&
                balance.SpellInfosById.TryGetValue(msg.SpellId, out SpellInfo spellInfo))
                world.PlayerManager.Player.StartCooldownDisplay(spellInfo);
        }

        public void Dispose() => subscription?.Dispose();
    }
}
