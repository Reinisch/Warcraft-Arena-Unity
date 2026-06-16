using System;
using Common;
using Core;
using Net;

namespace Client
{
    /// <summary>
    /// CLIENT-SIDE chat handling. Receives a broadcast <see cref="UnitChatMessage"/>, resolves the
    /// sender via <see cref="INetEntityRegistry"/>, and raises the existing
    /// <see cref="GameEvents.UnitChat"/> event — preserving what InputReference.Say used to fire inline.
    /// </summary>
    public sealed class ClientChatHandler : IDisposable
    {
        private readonly EventBus eventBus;
        private readonly INetEntityRegistry registry;
        private readonly IDisposable subscription;

        public ClientChatHandler(INetworkMessageBus bus, EventBus eventBus, INetEntityRegistry registry)
        {
            this.eventBus = eventBus;
            this.registry = registry;
            subscription = bus.Subscribe<UnitChatMessage>(OnChat);
        }

        private void OnChat(UnitChatMessage msg, NetContext ctx)
        {
            if (registry.TryGet(msg.SenderId, out Unit sender))
                eventBus.ExecuteEvent<Unit, string>(GameEvents.UnitChat, sender, msg.Message);
        }

        public void Dispose() => subscription?.Dispose();
    }
}
