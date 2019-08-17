using Bolt;
using Common;
using Core;
using UnityEngine;

namespace Server
{
    internal class GameSpellListener : BaseGameListener
    {
        internal GameSpellListener(WorldServerManager world) : base(world)
        {
            EventHandler.RegisterEvent<SpellDamageInfo>(EventHandler.GlobalDispatcher, GameEvents.ServerDamageDone, OnSpellDamageDone);
            EventHandler.RegisterEvent<Unit, Vector3, SpellInfo, IProtocolToken>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellLaunch, OnServerSpellLaunch);
            EventHandler.RegisterEvent<Unit, Unit, SpellInfo, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, OnServerSpellHit);
            EventHandler.RegisterEvent<Player, Vector3>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerTeleport, OnServerPlayerTeleport);
            EventHandler.RegisterEvent<Player, SpellCooldown>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCooldown, OnServerSpellCooldown);
        }

        internal override void Dispose()
        {
            EventHandler.UnregisterEvent<SpellDamageInfo>(EventHandler.GlobalDispatcher, GameEvents.ServerDamageDone, OnSpellDamageDone);
            EventHandler.UnregisterEvent<Unit, Vector3, SpellInfo, IProtocolToken>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellLaunch, OnServerSpellLaunch);
            EventHandler.UnregisterEvent<Unit, Unit, SpellInfo, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, OnServerSpellHit);
            EventHandler.UnregisterEvent<Player, Vector3>(EventHandler.GlobalDispatcher, GameEvents.ServerPlayerTeleport, OnServerPlayerTeleport);
            EventHandler.UnregisterEvent<Player, SpellCooldown>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCooldown, OnServerSpellCooldown);
        }

        private void OnSpellDamageDone(SpellDamageInfo damageInfo)
        {
            if (damageInfo.Caster is Player player && World.IsControlledByHuman(player))
            {
                SpellDamageDoneEvent spellDamageEvent = player.IsController
                    ? SpellDamageDoneEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered)
                    : SpellDamageDoneEvent.Create(player.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);

                spellDamageEvent.Target = damageInfo.Target.BoltEntity.NetworkId;
                spellDamageEvent.DamageAmount = (int)damageInfo.Damage;
                spellDamageEvent.IsCrit = damageInfo.HasCrit;
                spellDamageEvent.Send();
            }

            UnitSpellDamageEvent unitSpellDemageEvent = UnitSpellDamageEvent.Create(damageInfo.Target.BoltEntity, EntityTargets.Everyone);
            unitSpellDemageEvent.CasterId = damageInfo.Caster.BoltEntity.NetworkId;
            unitSpellDemageEvent.Damage = (int)damageInfo.Damage;
            unitSpellDemageEvent.IsCrit = damageInfo.HasCrit;
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
