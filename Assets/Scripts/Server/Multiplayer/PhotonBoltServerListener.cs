using Core;
using Bolt;

namespace Server
{
    public class PhotonBoltServerListener : PhotonBoltBaseListener
    {
        private new WorldServerManager WorldManager { get; set; }

        public void Initialize(WorldServerManager worldManager)
        {
            base.Initialize(worldManager);

            WorldManager = worldManager;
        }

        public new void Deinitialize()
        {
            WorldManager = null;

            base.Deinitialize();
        }

        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            if (WorldManager.HasClientLogic)
                WorldManager.CreatePlayer();
        }

        public override void SceneLoadRemoteDone(BoltConnection connection)
        {
            base.SceneLoadRemoteDone(connection);

            WorldManager.CreatePlayer(connection);
        }

        public override void Connected(BoltConnection boltConnection)
        {
            base.Connected(boltConnection);

            WorldManager.SetScope(boltConnection, true);
        }

        public override void Disconnected(BoltConnection boltConnection)
        {
            base.Disconnected(boltConnection);

            WorldManager.SetNetworkState(boltConnection, PlayerNetworkState.Disconnected);
        }

        public override void EntityAttached(BoltEntity entity)
        {
            base.EntityAttached(entity);

            WorldManager.EntityAttached(entity);
        }

        public override void EntityDetached(BoltEntity entity)
        {
            base.EntityDetached(entity);

            WorldManager.EntityDetached(entity);
        }

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
                spellCastAnswer.Result = (int) SpellCastResult.CasterNotExists;
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
