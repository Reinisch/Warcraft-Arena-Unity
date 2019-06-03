using Bolt;

namespace Core
{
    public abstract class WorldManager
    {
        private readonly EntityPool entityPool = new EntityPool();
        private readonly IPrefabPool defaultPool = new DefaultPrefabPool();

        public UnitManager UnitManager { get; }
        public MapManager MapManager { get; }

        public bool HasServerLogic { get; protected set; }
        public bool HasClientLogic { get; protected set; }

        protected WorldManager()
        {
            entityPool.Initialize(this);
            BoltNetwork.SetPrefabPool(entityPool);

            MapManager = new MapManager(this);
            UnitManager = new UnitManager();
        }

        public virtual void Dispose()
        {
            UnitManager.Dispose();
            MapManager.Dispose();

            BoltNetwork.SetPrefabPool(defaultPool);
            entityPool.Deinitialize();
        }

        public virtual void DoUpdate(int deltaTime)
        {
            MapManager.DoUpdate(deltaTime);
        }

        public Map FindMap(int mapId)
        {
            return MapManager.FindMap(mapId);
        }
    }
}