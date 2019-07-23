using Core;
using Bolt;

namespace Server
{
    public partial class PhotonBoltServerListener
    {
        public override void OnEvent(SpellCastRequestEvent spellCastRequest)
        {
            base.OnEvent(spellCastRequest);

            Player caster = World.FindPlayer(spellCastRequest.RaisedBy);
            SpellCastRequestAnswerEvent spellCastAnswer = spellCastRequest.FromSelf
                ? SpellCastRequestAnswerEvent.Create(GlobalTargets.OnlyServer)
                : SpellCastRequestAnswerEvent.Create(spellCastRequest.RaisedBy);

            spellCastAnswer.SpellId = spellCastRequest.SpellId;

            if (caster == null)
            {
                spellCastAnswer.Result = (int)SpellCastResult.CasterNotExists;
                spellCastAnswer.Send();
                return;
            }

            if (!balance.SpellInfosById.TryGetValue(spellCastRequest.SpellId, out SpellInfo spellInfo))
            {
                spellCastAnswer.Result = (int)SpellCastResult.SpellUnavailable;
                spellCastAnswer.Send();
                return;
            }

            SpellCastResult castResult = caster.Spells.CastSpell(spellInfo, new SpellCastingOptions(movementFlags: (MovementFlags)spellCastRequest.MovementFlags));
            if (castResult != SpellCastResult.Success)
            {
                spellCastAnswer.Result = (int)castResult;
                spellCastAnswer.Send();
            }
        }

        public override void OnEvent(SpellCastCancelRequestEvent spellCancelRequest)
        {
            base.OnEvent(spellCancelRequest);

            Player caster = World.FindPlayer(spellCancelRequest.RaisedBy);
            if (caster != null && caster.SpellCast.IsCasting)
                caster.SpellCast.Cancel();
        }
    }
}
