using Bolt;
using Common;
using Core;

namespace Server
{
    internal class GameSpellListener : BaseGameListener
    {
        internal GameSpellListener(WorldServerManager worldServerManager) : base(worldServerManager)
        {
            EventHandler.RegisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
            EventHandler.RegisterEvent<Unit, SpellInfo>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCast, OnServerSpellCast);
            EventHandler.RegisterEvent<Unit, Unit, SpellInfo, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, OnServerSpellHit);
        }

        internal override void Dispose()
        {
            EventHandler.UnregisterEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, GameEvents.SpellDamageDone, OnSpellDamageDone);
            EventHandler.UnregisterEvent<Unit, SpellInfo>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellCast, OnServerSpellCast);
            EventHandler.UnregisterEvent<Unit, Unit, SpellInfo, SpellMissType>(EventHandler.GlobalDispatcher, GameEvents.ServerSpellHit, OnServerSpellHit);
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

        private void OnServerSpellCast(Unit caster, SpellInfo spellInfo)
        {
            UnitSpellCastEvent unitCastEvent = UnitSpellCastEvent.Create(caster.BoltEntity, EntityTargets.EveryoneExceptController);
            unitCastEvent.SpellId = spellInfo.Id;
            unitCastEvent.Send();

            SpellCastRequestAnswerEvent spellCastAnswer = caster.IsController
                ? SpellCastRequestAnswerEvent.Create(GlobalTargets.OnlyServer, ReliabilityModes.ReliableOrdered)
                : SpellCastRequestAnswerEvent.Create(caster.BoltEntity.Controller, ReliabilityModes.ReliableOrdered);

            spellCastAnswer.SpellId = spellInfo.Id;
            spellCastAnswer.Result = (int) SpellCastResult.Success;
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
    }
}
