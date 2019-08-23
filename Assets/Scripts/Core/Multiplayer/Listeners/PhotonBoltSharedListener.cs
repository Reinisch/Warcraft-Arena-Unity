using JetBrains.Annotations;

namespace Core
{
    [UsedImplicitly]
    public class PhotonBoltSharedListener : PhotonBoltBaseListener
    {
        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            if (BoltNetwork.IsConnected)
                World.MapManager.InitializeLoadedMap(1);
        }
    }
}
