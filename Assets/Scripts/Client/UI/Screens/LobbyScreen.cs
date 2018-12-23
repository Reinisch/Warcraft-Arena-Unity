using Core;
using JetBrains.Annotations;
using UdpKit;
using UnityEngine;

namespace Client
{
    public class LobbyScreen : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private LobbyPanel lobbyPanel;

        public void Initialize(PhotonBoltManager photonManager)
        {
            gameObject.SetActive(false);

            lobbyPanel.Initialize(photonManager, this);
        }

        public void Deinitialize()
        {
            gameObject.SetActive(false);

            lobbyPanel.Deinitialize();
        }

        public void Show(bool autoStartClient)
        {
            gameObject.SetActive(true);

            lobbyPanel.Show(autoStartClient);
        }

        public void Hide()
        {
            lobbyPanel.Hide();

            gameObject.SetActive(false);
        }

        public void SetStatusDisconnectDescription(UdpConnectionDisconnectReason reason)
        {
            lobbyPanel.SetStatusDisconnectDescription(reason);
        }
    }
}
