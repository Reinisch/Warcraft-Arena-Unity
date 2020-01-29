using Common;
using Core;

using EventHandler = Common.EventHandler;

namespace Client
{
    public partial class PhotonBoltClientListener
    {
        public override void OnEvent(SpellCastRequestAnswerEvent answer)
        {
            base.OnEvent(answer);

            if (answer.Result == (int) SpellCastResult.Success)
            {
                var token = answer.ProcessingEntries as SpellProcessingToken;
                EventHandler.ExecuteEvent(GameEvents.SpellLaunched, (Unit)LocalPlayer, answer.SpellId, token);
            }
            else
                EventHandler.ExecuteEvent(GameEvents.ClientSpellFailed, (SpellCastResult) answer.Result);
        }

        public override void OnEvent(SpellDamageDoneEvent spellDamageEvent)
        {
            base.OnEvent(spellDamageEvent);

            if (LocalPlayer == null || !World.UnitManager.TryFind(spellDamageEvent.Target.PackedValue, out Unit target))
                return;

            EventHandler.ExecuteEvent(GameEvents.SpellDamageDone, (Unit)LocalPlayer, target, spellDamageEvent.DamageAmount, (HitType)spellDamageEvent.HitType);
        }

        public override void OnEvent(SpellMissDoneEvent spellMissEvent)
        {
            base.OnEvent(spellMissEvent);

            if (LocalPlayer == null || !World.UnitManager.TryFind(spellMissEvent.TargetId.PackedValue, out Unit target))
                return;

            EventHandler.ExecuteEvent(GameEvents.SpellMissDone, (Unit)LocalPlayer, target, (SpellMissType)spellMissEvent.MissType);
        }

        public override void OnEvent(SpellHealingDoneEvent spellHealingEvent)
        {
            base.OnEvent(spellHealingEvent);

            if (LocalPlayer == null || !World.UnitManager.TryFind(spellHealingEvent.Target.PackedValue, out Unit target))
                return;

            EventHandler.ExecuteEvent(GameEvents.SpellHealingDone, (Unit)LocalPlayer, target, spellHealingEvent.HealAmount, spellHealingEvent.IsCrit);
        }

        public override void OnEvent(SpellPlayerTeleportEvent teleportEvent)
        {
            base.OnEvent(teleportEvent);

            LocalPlayer?.Handle(teleportEvent);
        }

        public override void OnEvent(SpellCooldownEvent cooldownEvent)
        {
            base.OnEvent(cooldownEvent);

            if (LocalPlayer.ExistsIn(World))
                LocalPlayer.SpellHistory.Handle(cooldownEvent);
        }

        public override void OnEvent(SpellChargeEvent chargeEvent)
        {
            base.OnEvent(chargeEvent);

            if (LocalPlayer.ExistsIn(World))
                LocalPlayer.SpellHistory.Handle(chargeEvent);
        }
    }
}
