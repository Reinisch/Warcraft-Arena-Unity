using Core;
using JetBrains.Annotations;
using UnityEngine;
using Client.UI;

namespace Client
{
    public class LobbyScreen : UIWindowController<LobbyPanelType>
    {
        [SerializeField, UsedImplicitly] private LobbyPanel lobbyPanel;

        public void Initialize(PhotonBoltManager photonManager)
        {
            gameObject.SetActive(false);

            RegisterPanel(lobbyPanel, new LobbyPanel.InitData(photonManager, this));
        }

        public void Deinitialize()
        {
            UnregisterPanel(lobbyPanel, new UIWindow<LobbyPanelType>.DefaultDeinitData());

            gameObject.SetActive(false);
        }
    }
}
