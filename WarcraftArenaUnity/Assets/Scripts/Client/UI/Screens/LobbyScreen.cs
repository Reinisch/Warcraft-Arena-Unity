using JetBrains.Annotations;
using UnityEngine;
using Client.UI;

namespace Client
{
    public class LobbyScreen : UIWindowController<LobbyScreen>
    {
        [SerializeField, UsedImplicitly] private LobbyPanel lobbyPanel;

        public new void Initialize(ScreenController controller)
        {
            base.Initialize(controller);

            gameObject.SetActive(false);

            RegisterPanel<LobbyPanel, LobbyPanel.RegisterToken>(lobbyPanel);
        }

        public new void Deinitialize(ScreenController controller)
        {
            UnregisterPanel<LobbyPanel, LobbyPanel.UnregisterToken>(lobbyPanel);

            gameObject.SetActive(false);

            base.Deinitialize(controller);
        }
    }
}
