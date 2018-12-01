using Core;

namespace Client
{
    public class InputManager : SingletonGameObject<InputManager>
    {
        public Player OriginalPlayer { get; private set; }

        public void Initialize(WorldManager worldManager)
        {
            OriginalPlayer = MapManager.Instance.FindMap(1).FindMapEntity<Player>(worldManager.LocalPlayerId);
        }

        public void Deinitialize()
        {
            OriginalPlayer = null;
        }
    }
}
