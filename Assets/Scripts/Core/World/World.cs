using Bolt;

namespace Core
{
    public abstract class World
    {
        private readonly EntityPool entityPool = new EntityPool();
        private readonly IPrefabPool defaultPool = new DefaultPrefabPool();

        internal SpellManager SpellManager { get; }
        internal MapManager MapManager { get; }

        public UnitManager UnitManager { get; }

        public bool HasServerLogic { get; protected set; }
        public bool HasClientLogic { get; protected set; }

        protected World()
        {
            entityPool.Initialize(this);
            BoltNetwork.SetPrefabPool(entityPool);

            UnitManager = new UnitManager();
            MapManager = new MapManager(this);
            SpellManager = new SpellManager(this);
        }

        public virtual void Dispose()
        {
            SpellManager.Dispose();
            UnitManager.Dispose();
            MapManager.Dispose();

            BoltNetwork.SetPrefabPool(defaultPool);
            entityPool.Deinitialize();
        }

        public virtual void DoUpdate(int deltaTime)
        {
            MapManager.DoUpdate(deltaTime);

            UnitManager.DoUpdate(deltaTime);

            if(HasServerLogic)
                SpellManager.DoUpdate(deltaTime);
        }

        public Map FindMap(int mapId)
        {
            return MapManager.FindMap(mapId);
        }
    }
}