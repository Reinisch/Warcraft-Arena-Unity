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
            else
                EventHandler.ExecuteEvent(EventHandler.GlobalDispatcher, GameEvents.ClientSpellFailed, (SpellCastResult) answer.Result);
        }

        public override void OnEvent(SpellDamageDoneEvent spellDamageEvent)
        {
            base.OnEvent(spellDamageEvent);

            if (LocalPlayer == null || !WorldManager.UnitManager.TryFind(spellDamageEvent.Target.PackedValue, out Unit target))
                return;

            EventHandler.ExecuteEvent<Unit, Unit, int, bool>(EventHandler.GlobalDispatcher, 
                GameEvents.SpellDamageDone, LocalPlayer, target, spellDamageEvent.DamageAmount, spellDamageEvent.IsCrit);
        }

        public override void OnEvent(SpellPlayerTeleportEvent teleportEvent)
        {
            base.OnEvent(teleportEvent);

            if (LocalPlayer != null)
            {
                LocalPlayer.Position = teleportEvent.TargetPosition;
                LocalPlayer.MovementInfo.RemoveMovementFlag(MovementFlags.Ascending);
            }
        }
    }
}
