using Common;
using Core;

namespace Client
{
    public class InputManager : SingletonBehaviour<InputManager>
    {
        public Player OriginalPlayer { get; private set; }

        private WorldManager worldManager;

        public new void Initialize()
        {
            base.Initialize();
        }

        public new void Deinitialize()
        {
            base.Deinitialize();
        }

        public void InitializeWorld(WorldManager worldManager)
        {
            this.worldManager = worldManager;

            worldManager.UnitManager.EventEntityAttached += OnEventEntityAttached;
            worldManager.UnitManager.EventEntityDetach += OnEventEntityDetach;
        }

        public void DeinitializeWorld()
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
