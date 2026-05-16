using Bolt;
using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class PhotonBoltSharedListener : PhotonBoltBaseListener
    {
        [SerializeField, UsedImplicitly] PhotonBoltReference photon;

        public override void SceneLoadLocalDone(string scene, IProtocolToken token)
        {
            base.SceneLoadLocalDone(scene, token);

            if (BoltNetwork.IsConnected && BoltNetwork.IsClient && token is ServerRoomToken roomToken)
                World.MapManager.InitializeLoadedMap(1, roomToken.Scenario);
        }
    }
}
