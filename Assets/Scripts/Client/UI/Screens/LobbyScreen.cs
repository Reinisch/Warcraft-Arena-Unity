using Core;
using JetBrains.Annotations;
using UnityEngine;
using Client.UI;

namespace Client
{
    public class LobbyScreen : UIWindowController
    {
        [SerializeField, UsedImplicitly] private LobbyPanel lobbyPanel;

        public void Initialize(PhotonBoltManager photonManager, ScreenController controller)
        {
            Initialize(controller);

            gameObject.SetActive(false);

            RegisterPanel(lobbyPanel, new LobbyPanel.RegisterToken(photonManager, this));
        }

        public new void Deinitialize(ScreenController controller)
        {
            UnregisterPanel(lobbyPanel, new LobbyPanel.UnregisterToken());

            gameObject.SetActive(false);

            base.Deinitialize(controller);
        }
    }
}
