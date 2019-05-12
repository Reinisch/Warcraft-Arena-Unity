using Core;

namespace Client
{
    public class InputManager : SingletonGameObject<InputManager>
    {
        public Player OriginalPlayer { get; private set; }

        private WorldManager worldManager;

        public void Initialize(WorldManager worldManager)
        {
            this.worldManager = worldManager;

            worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
            worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;
        }

        public void Deinitialize()
        {
            worldManager.UnitManager.EventEntityAttached -= OnEventEntityAttached;
            worldManager.UnitManager.EventEntityDetach -= OnEventEntityDetach;

            worldManager = null;
        }

        private void OnEventEntityAttached(WorldEntity worldEntity)
        {
            if (worldEntity is Player player && player.IsOwner)
                OriginalPlayer = player;
        }

        private void OnEventEntityDetach(WorldEntity worldEntity)
        {
            if (worldEntity == OriginalPlayer)
                OriginalPlayer = null;
        }
    }
}
