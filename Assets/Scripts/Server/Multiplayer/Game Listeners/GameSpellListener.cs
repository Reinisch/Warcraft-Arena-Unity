using Bolt;
using Common;
using Core;
using UnityEngine;

namespace Server
{
    internal class GameSpellListener : BaseGameListener
    {
        internal GameSpellListener(WorldServerManager worldServerManager) : base(worldServerManager)
        {
            EventHandler.RegisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
            EventHandler.RegisterEvent<Unit, Vector3, SpellInfo, IProtocolToken>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellLaunch, OnServerSpellLaunch);
            EventHandler.RegisterEvent<Unit, Unit, SpellInfo, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, OnServerSpellHit);
            EventHandler.RegisterEvent<Player, Vector3>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerTeleport, OnServerPlayerTeleport);
            EventHandler.RegisterEvent<Player, SpellCooldown>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCooldown, OnServerSpellCooldown);
        }

        internal override void Dispose()
        {
            EventHandler.UnregisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
            EventHandler.UnregisterEvent<Unit, Vector3, SpellInfo, IProtocolToken>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellLaunch, OnServerSpellLaunch);
            EventHandler.UnregisterEvent<Unit, Unit, SpellInfo, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, OnServerSpellHit);
            EventHandler.UnregisterEvent<Player, Vector3>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerTeleport, OnServerPlayerTeleport);
            EventHandler.UnregisterEvent<Player, SpellCooldown>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCooldown, OnServerSpellCooldown);
        }

        private void OnSpellDamageDone(Unit caster, Unit target, int damageAmount, bool isCrit)
        {
            if (caster is Player player && player.BoltEntity.Controller != null)
            {
                SpellDamageDoneEvent spellDamageEvent = SpellDamageDoneEvent.Create(player.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);
                spellDamageEvent.Target = target.BoltEntity.NetworkId;
                spellDamageEvent.DamageAmount = damageAmount;
                spellDamageEvent.IsCrit = isCrit;
                spellDamageEvent.Send();
            }

            UnitSpellDamageEvent unitSpellDemageEvent = UnitSpellDamageEvent.Create(target.BoltEntity, EntityTargets.Everyone);
            unitSpellDemageEvent.CasterId = caster.BoltEntity.NetworkId;
            unitSpellDemageEvent.Damage = damageAmount;
            unitSpellDemageEvent.IsCrit = isCrit;
            unitSpellDemageEvent.Send();
        }

        private void OnServerSpellLaunch(Unit caster, Vector3 source, SpellInfo spellInfo, IProtocolToken processingToken)
        {
            UnitSpellLaunchEvent unitCastEvent = UnitSpellLaunchEvent.Create(caster.BoltEntity, EntityTargets.Everyone);
            unitCastEvent.SpellId = spellInfo.Id;
            unitCastEvent.ProcessingEntries = processingToken;
            unitCastEvent.Source = source;
            unitCastEvent.Send();

            SpellCastRequestAnswerEvent spellCastAnswer = caster.IsController
                ? SpellCastRequestAnswerEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered)
                : SpellCastRequestAnswerEvent.Create(caster.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);

            spellCastAnswer.SpellId = spellInfo.Id;
            spellCastAnswer.Result = (int) SpellCastResult.Success;
            spellCastAnswer.ProcessingEntries = processingToken;
            spellCastAnswer.Source = source;
            spellCastAnswer.Send();
        }

        private void OnServerSpellHit(Unit caster, Unit target, SpellInfo spellInfo, SpellMissType missType)
        {
            UnitSpellHitEvent unitSpellHitEvent = UnitSpellHitEvent.Create(target.BoltEntity, EntityTargets.Everyone);
            unitSpellHitEvent.CasterId = caster.BoltEntity.NetworkId;
            unitSpellHitEvent.SpellId = spellInfo.Id;
            unitSpellHitEvent.MissType = (int) missType;
            unitSpellHitEvent.Send();
        }

        private void OnServerPlayerTeleport(Player player, Vector3 targetPosition)
        {
            if (player.BoltEntity.Controller != null)
            {
                SpellPlayerTeleportEvent spellTeleportEvent = SpellPlayerTeleportEvent.Create(player.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);
                spellTeleportEvent.TargetPosition = targetPosition;
                spellTeleportEvent.Send();
            }
        }

        private void OnServerSpellCooldown(Player player, SpellCooldown cooldown)
        {
            if (player.BoltEntity.Controller != null)
            {
                SpellCooldownEvent spellCooldownEvent = SpellCooldownEvent.Create(player.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);
                spellCooldownEvent.SpellId = cooldown.SpellId;
                spellCooldownEvent.CooldownTime = cooldown.Cooldown;
                spellCooldownEvent.ServerFrame = BoltNetwork.ServerFrame;
                spellCooldownEvent.Send();
            }
        }
    }
}
