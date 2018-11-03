namespace Core
{
    public class World
    {
        public static World Instance { get; } = new World();

        public void Initialize()
        {
            MapManager.Initialize();
            MapManager.Instance.CreateBaseMap(1);
        }

        public void Deinitialize()
        {
            MapManager.Deinitialize();
        }

        public void DoUpdate(int deltaTime)
        {
            MapManager.Instance.DoUpdate(deltaTime);
        }

        public static Map FindMap(int mapId)
        {
            return MapManager.Instance.FindMap(mapId);
        }
    }
}