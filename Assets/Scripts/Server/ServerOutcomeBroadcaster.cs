using System;
using System.Collections.Generic;
using Common;
using Core;
using Net;
using UnityEngine;

namespace Server
{
    /// <summary>
    /// Server-authoritative outcome bridge (the old project's GameSpellListener equivalent). Subscribes to
    /// Core's gameplay events and sends the matching network messages to clients.
    ///
    /// Only sends when this instance has server logic. On a host these sends reach the local
    /// <see cref="Client.ClientOutcomeHandler"/> but are suppressed there (the host already rendered them
    /// directly), so there's no double-fire; remote clients consume them.
    /// </summary>
    public sealed class ServerOutcomeBroadcaster : IDisposable
    {
        private readonly INetworkMessageBus bus;
        private readonly EventBus eventBus;
        private readonly INetEntityRegistry registry;
        private readonly World world;

        public ServerOutcomeBroadcaster(INetworkMessageBus bus, EventBus eventBus, INetEntityRegistry registry, World world)
        {
            this.bus = bus;
            this.eventBus = eventBus;
            this.registry = registry;
            this.world = world;

            eventBus.RegisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnLaunch);
            eventBus.RegisterEvent<Unit, Unit, int, HitType, Vector3?>(GameEvents.SpellDamageDone, OnDamage);
            eventBus.RegisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnHeal);
            eventBus.RegisterEvent<Unit, Unit, SpellMissType>(GameEvents.SpellMissDone, OnMiss);
            eventBus.RegisterEvent<Unit, int>(GameEvents.SpellHit, OnHit);
        }

        private void OnLaunch(Unit caster, int spellId, SpellProcessingToken token)
        {
            if (!world.HasServerLogic)
                return;

            // Translate the Core token's target ids (server-side Entity.Ids) to NetIds for the wire; the
            // client maps them back to its own units.
            var targets = new List<SpellLaunchTarget>(token.ProcessingEntries.Count);
            foreach ((ulong entityId, float time) in token.ProcessingEntries)
                if (world.UnitManager.TryFind(entityId, out Unit target))
                    targets.Add(new SpellLaunchTarget(registry.GetId(target), time));

            NetId casterId = registry.GetId(caster);
            bus.Send(new UnitSpellLaunch(casterId, spellId, token.Source, token.Destination, targets),
                NetTarget.Observers(casterId));
        }

        private void OnDamage(Unit caster, Unit target, int damage, HitType hitType, Vector3? hitPosition)
        {
            if (!world.HasServerLogic)
                return;

            bus.Send(
                new SpellDamageDone(registry.GetId(caster), registry.GetId(target), damage, hitType,
                    hitPosition ?? Vector3.zero, hitPosition.HasValue),
                NetTarget.Observers(registry.GetId(target)));
        }

        private void OnHeal(Unit healer, Unit target, int heal, bool isCrit)
        {
            if (!world.HasServerLogic)
                return;

            bus.Send(
                new SpellHealingDone(registry.GetId(healer), registry.GetId(target), heal, isCrit),
                NetTarget.Observers(registry.GetId(target)));
        }

        private void OnMiss(Unit caster, Unit target, SpellMissType missType)
        {
            if (!world.HasServerLogic)
                return;

            bus.Send(
                new SpellMissDone(registry.GetId(caster), registry.GetId(target), missType),
                NetTarget.Observers(registry.GetId(target)));
        }

        private void OnHit(Unit target, int spellId)
        {
            if (!world.HasServerLogic)
                return;

            bus.Send(new UnitSpellHit(registry.GetId(target), spellId), NetTarget.Observers(registry.GetId(target)));
        }

        public void Dispose()
        {
            eventBus.UnregisterEvent<Unit, int, SpellProcessingToken>(GameEvents.SpellLaunched, OnLaunch);
            eventBus.UnregisterEvent<Unit, Unit, int, HitType, Vector3?>(GameEvents.SpellDamageDone, OnDamage);
            eventBus.UnregisterEvent<Unit, Unit, int, bool>(GameEvents.SpellHealingDone, OnHeal);
            eventBus.UnregisterEvent<Unit, Unit, SpellMissType>(GameEvents.SpellMissDone, OnMiss);
            eventBus.UnregisterEvent<Unit, int>(GameEvents.SpellHit, OnHit);
        }
    }
}
