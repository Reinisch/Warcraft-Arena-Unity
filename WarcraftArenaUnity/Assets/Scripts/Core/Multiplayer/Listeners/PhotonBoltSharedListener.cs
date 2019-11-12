using JetBrains.Annotations;
using UnityEngine;

namespace Core
{
    [UsedImplicitly]
    public class PhotonBoltSharedListener : PhotonBoltBaseListener
    {
        [SerializeField, UsedImplicitly] PhotonBoltReference photon;

        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            if (BoltNetwork.IsConnected && BoltNetwork.IsClient)
                World.MapManager.InitializeLoadedMap(1);
        }
    }
}
