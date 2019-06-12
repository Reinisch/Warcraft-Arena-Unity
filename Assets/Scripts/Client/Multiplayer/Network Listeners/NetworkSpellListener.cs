using Common;
using Core;

using EventHandler = Common.EventHandler;

namespace Client
{
    public partial class PhotonBoltClientListener
    {
        public override void OnEvent(SpellCastRequestAnswerEvent spellCastAnswer)
        {
            base.OnEvent(spellCastAnswer);

            if (spellCastAnswer.Result == (int)SpellCastResult.Success)
                EventHandler.ExecuteEvent<Unit, int>(EventHandler.GlobalDispatcher, GameEvents.SpellCasted, LocalPlayer, spellCastAnswer.SpellId);
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
