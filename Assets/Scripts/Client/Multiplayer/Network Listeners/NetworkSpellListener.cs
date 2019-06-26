using Bolt;
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
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.SpellLaunched, (Unit)LocalPlayer, answer.SpellId, token);
            }
        }

        public override void OnEvent(SpellDamageDoneEvent spellDamageEvent)
        {
            base.OnEvent(spellDamageEvent);

            if (LocalPlayer == null || !WorldManager.UnitManager.TryGet(spellDamageEvent.Target.PackedValue, out Unit target))
                return;

            EventHandler.ExecuteEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, 
                GameEvents.SpellDamageDone, LocalPlayer, target, spellDamageEvent.DamageAmount, spellDamageEvent.IsCrit);
        }
    }
}
