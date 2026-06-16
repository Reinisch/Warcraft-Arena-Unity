using System;
using Common;
using Core;
using Net;
using UnityEngine;

namespace Client
{
    /// <summary>
    /// CLIENT-SIDE outcome bridge. Receives gameplay outcome messages and re-raises the same local Core
    /// events (SpellDamageDone / SpellHealingDone / SpellMissDone / SpellHit) so existing renderers and
    /// sound stay untouched.
    ///
    /// Active only on a pure remote client. A host/server already gets these directly from the in-process
    /// World, so re-raising there would double-fire — hence the <see cref="IsRemoteClient"/> guard.
    /// </summary>
    public sealed class ClientOutcomeHandler : IDisposable
    {
        private readonly EventBus eventBus;
        private readonly INetEntityRegistry registry;
        private readonly World world;
        private readonly IDisposable[] subscriptions;

        public ClientOutcomeHandler(INetworkMessageBus bus, EventBus eventBus, INetEntityRegistry registry, World world)
        {
            this.eventBus = eventBus;
            this.registry = registry;
            this.world = world;

            subscriptions = new[]
            {
                bus.Subscribe<SpellDamageDone>(OnDamage),
                bus.Subscribe<SpellHealingDone>(OnHeal),
                bus.Subscribe<SpellMissDone>(OnMiss),
                bus.Subscribe<UnitSpellHit>(OnHit),
                bus.Subscribe<UnitSpellLaunch>(OnLaunch),
            };
        }

        private bool IsRemoteClient => world.HasClientLogic && !world.HasServerLogic;

        private void OnDamage(SpellDamageDone msg, NetContext ctx)
        {
            if (!IsRemoteClient || !registry.TryGet(msg.TargetId, out Unit target))
                return;

            registry.TryGet(msg.CasterId, out Unit caster);
            Vector3? hitPosition = msg.HasHitPosition ? msg.HitPosition : (Vector3?)null;
            eventBus.ExecuteEvent(GameEvents.SpellDamageDone, caster, target, msg.Damage, msg.HitType, hitPosition);
        }

        private void OnHeal(SpellHealingDone msg, NetContext ctx)
        {
            if (!IsRemoteClient || !registry.TryGet(msg.TargetId, out Unit target))
                return;

            registry.TryGet(msg.HealerId, out Unit healer);
            eventBus.ExecuteEvent(GameEvents.SpellHealingDone, healer, target, msg.Heal, msg.IsCrit);
        }

        private void OnMiss(SpellMissDone msg, NetContext ctx)
        {
            if (!IsRemoteClient || !registry.TryGet(msg.TargetId, out Unit target))
                return;

            registry.TryGet(msg.CasterId, out Unit caster);
            eventBus.ExecuteEvent(GameEvents.SpellMissDone, caster, target, msg.MissType);
        }

        private void OnHit(UnitSpellHit msg, NetContext ctx)
        {
            if (!IsRemoteClient || !registry.TryGet(msg.TargetId, out Unit target))
                return;

            eventBus.ExecuteEvent(GameEvents.SpellHit, target, msg.SpellId);
        }

        private void OnLaunch(UnitSpellLaunch msg, NetContext ctx)
        {
            if (!IsRemoteClient || !registry.TryGet(msg.CasterId, out Unit caster))
                return;

            // Rebuild a local Core token: translate target NetIds back to this client's units (their Core
            // Entity.Ids are what the renderer's projectile lookup expects).
            var token = new SpellProcessingToken(msg.Source, msg.Destination);
            if (msg.Targets != null)
                foreach (SpellLaunchTarget target in msg.Targets)
                    if (registry.TryGet(target.TargetId, out Unit targetUnit))
                        token.ProcessingEntries.Add((targetUnit.Id, target.Time));

            eventBus.ExecuteEvent(GameEvents.SpellLaunched, caster, msg.SpellId, token);
        }

        public void Dispose()
        {
            for (int i = 0; i < subscriptions.Length; i++)
                subscriptions[i]?.Dispose();
        }
    }
}
