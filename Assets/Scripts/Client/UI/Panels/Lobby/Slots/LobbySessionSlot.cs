using System;
using Bolt.Utils;
using Core;
using JetBrains.Annotations;
using TMPro;
using UdpKit;
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

        public event Action<LobbySessionSlot> EventLobbySessionSlotSelected;

        public UdpSession UdpSession { get; private set; }

        public void Initialize()
        {
            slotButton.onClick.AddListener(OnSessionSlotClicked);

            SetSession(null);
        }

        public void Deinitialize()
        {
            slotButton.onClick.RemoveListener(OnSessionSlotClicked);
        }

        public void SetSession(UdpSession updSession)
        {
            UdpSession = updSession;

            if (UdpSession?.GetProtocolToken() is ServerRoomToken serverRoomToken)
            {
                mapNameLabel.text = serverRoomToken.Map;
                serverNameLabel.text = serverRoomToken.Name;
                versionNameLabel.text = serverRoomToken.Version;
                gameObject.SetActive(true);
            }
            else
                gameObject.SetActive(false);
        }

        private void OnSessionSlotClicked()
        {
            EventLobbySessionSlotSelected?.Invoke(this);
        }
    }
}
