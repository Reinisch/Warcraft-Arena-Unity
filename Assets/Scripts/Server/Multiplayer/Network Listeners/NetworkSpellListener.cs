using Core;
using Bolt;

namespace Server
{
    public partial class PhotonBoltServerListener
    {
        public override void OnEvent(SpellCastRequestEvent spellCastRequest)
        {
            base.OnEvent(spellCastRequest);

            Player caster = WorldManager.FindPlayer(spellCastRequest.RaisedBy);
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

            if (!BalanceManager.SpellInfosById.TryGetValue(spellCastRequest.SpellId, out SpellInfo spellInfo))
            {
                spellCastAnswer.Result = (int)SpellCastResult.SpellUnavailable;
                spellCastAnswer.Send();
                return;
            }

            SpellCastResult castResult = caster.CastSpell(new SpellCastTargets(), spellInfo);

            if (castResult == SpellCastResult.Success)
            {
                UnitSpellCastEvent unitCastEvent = UnitSpellCastEvent.Create(caster.BoltEntity, EntityTargets.EveryoneExceptController);
                unitCastEvent.SpellId = spellCastRequest.SpellId;
                unitCastEvent.Send();
            }

            spellCastAnswer.Result = (int)castResult;
            spellCastAnswer.Send();
        }
    }
}
