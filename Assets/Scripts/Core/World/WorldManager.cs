using Bolt;

namespace Core
{
    public abstract class WorldManager
    {
        private readonly EntityPool entityPool = new EntityPool();

        public EntityManager<WorldEntity> WorldEntityManager { get; } = new EntityManager<WorldEntity>();

        public virtual ulong LocalPlayerId => 0;

        public virtual void Initialize()
        {
            entityPool.Initialize(this);
            BoltNetwork.SetPrefabPool(entityPool);

            MapManager.Initialize();
            MapManager.Instance.CreateBaseMap(1);

            WorldEntityManager.Initialize();
        }

        public virtual void Deinitialize()
        {
            WorldEntityManager.Deinitialize();

            MapManager.Deinitialize();

            entityPool.Deinitialize();
            BoltNetwork.SetPrefabPool(new DefaultPrefabPool());
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