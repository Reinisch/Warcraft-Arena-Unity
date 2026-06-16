using System;
using JetBrains.Annotations;
using Net;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client
{
    /// <summary>
    /// One discoverable session row in the lobby's session list — display + a select/Join button. Mirrors
    /// <see cref="LobbyMapSlot"/>: instantiated from a disabled prototype and bound to a <see cref="SessionInfo"/>.
    /// Labels are null-guarded so the prefab can show as much or as little of the session as it likes.
    /// </summary>
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
            // Tag the name with its source so a single list distinguishes LAN hosts from Unity Lobby hosts.
            string sourceTag = session.Source == SessionSource.UnityServices ? "Online" : "LAN";
            if (serverNameLabel != null) serverNameLabel.text = $"{session.HostName} [{sourceTag}]";
            if (mapNameLabel != null) mapNameLabel.text = session.Map;
            if (versionNameLabel != null) versionNameLabel.text = session.Version;
            if (playerCountLabel != null) playerCountLabel.text = $"{session.PlayerCount}/{session.MaxPlayers}";

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
