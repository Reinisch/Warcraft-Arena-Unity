using System;

namespace Core
{
    public abstract class WorldManager
    {
        public virtual Guid LocalPlayerId => Guid.Empty;

        public virtual void Initialize()
        {
            MapManager.Initialize();
            MapManager.Instance.CreateBaseMap(1);
        }

        public virtual void Deinitialize()
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