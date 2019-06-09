namespace Core
{
    public class PhotonBoltSharedListener : PhotonBoltBaseListener
    {
        public new void Initialize(WorldManager worldManager)
        {
            base.Initialize(worldManager);
        }

        public new void Deinitialize()
        {
            base.Deinitialize();
        }

        public override void SceneLoadLocalDone(string map)
        {
            base.SceneLoadLocalDone(map);

            WorldManager.MapManager.InitializeLoadedMap(1);
        }
    }
}
