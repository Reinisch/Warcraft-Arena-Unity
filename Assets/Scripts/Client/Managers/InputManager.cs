using Core;

namespace Client
{
    public class InputManager : SingletonGameObject<InputManager>
    {
        public Player OriginalPlayer { get; private set; }

        public override void Initialize()
        {
            base.Initialize();

            if (GameManager.Instance.IsDebugLogic)
                OriginalPlayer = MapManager.Instance.FindMap(1).FindMapEntity<Player>(GameManager.Instance.LocalPlayerId);
        }

        public override void Deinitialize()
        {
            OriginalPlayer = null;

            base.Deinitialize();
        }
    }
}
