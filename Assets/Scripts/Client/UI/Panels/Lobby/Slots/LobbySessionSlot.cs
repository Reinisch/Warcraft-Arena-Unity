using System;
using JetBrains.Annotations;
using Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    public class LobbySessionSlot : MonoBehaviour
    {
        [SerializeField, UsedImplicitly] private Button slotButton;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI mapNameLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI serverNameLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI versionNameLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI playerCountLabel;

        public event Action<LobbySessionSlot> EventLobbySessionSlotSelected;

        public SessionInfo Session { get; private set; }

        public void Initialize(SessionInfo session)
        {
            gameObject.SetActive(true);

            Session = session;
            string sourceTag = session.Source == SessionSource.UnityServices ? "Online" : "LAN";
            if (serverNameLabel != null) serverNameLabel.text = $"{session.HostName} [{sourceTag}]";
            if (mapNameLabel != null) mapNameLabel.text = session.Map;
            if (versionNameLabel != null) versionNameLabel.text = session.Version;
            if (playerCountLabel != null)
                playerCountLabel.text = session.TeamSize > 0
                    ? $"{session.PlayerCount}/{session.MaxPlayers}   {session.TeamSize} vs {session.TeamSize}"
                    : $"{session.PlayerCount}/{session.MaxPlayers}";

            slotButton.onClick.AddListener(OnSessionSlotClicked);
        }

        public void Deinitialize()
        {
            Session = default;
            slotButton.onClick.RemoveListener(OnSessionSlotClicked);
        }

        private void OnSessionSlotClicked() => EventLobbySessionSlotSelected?.Invoke(this);
    }
}
