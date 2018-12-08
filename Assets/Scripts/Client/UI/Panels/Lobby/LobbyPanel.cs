using System.Collections.Generic;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UdpKit;

namespace Client
{
    public class LobbyPanel : Panel
    {
        [SerializeField, UsedImplicitly] private Button startServerButton;
        [SerializeField, UsedImplicitly] private Button clientServerButton;
        [SerializeField, UsedImplicitly] private Transform mapsContentHolder;
        [SerializeField, UsedImplicitly] private Transform sessionsContentHolder;
        [SerializeField, UsedImplicitly] private LobbyMapSlot mapSlotPrototype;
        [SerializeField, UsedImplicitly] private LobbySessionSlot sessionSlotPrototype;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI selectedMapLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI serverNameInput;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI statusLabel;

        private readonly List<LobbyMapSlot> mapSlots = new List<LobbyMapSlot>();
        private readonly List<LobbySessionSlot> sessionSlots = new List<LobbySessionSlot>();
        private const int SessionDisplayCount = 20;

        private LobbyScreen lobbyScreen;
        private LobbyMapSlot selectedMapSlot;
        private PhotonBoltManager photonManager;

        public void Initialize(PhotonBoltManager photonManager, LobbyScreen lobbyScreen)
        {
            this.photonManager = photonManager;
            this.lobbyScreen = lobbyScreen;

            startServerButton.onClick.AddListener(OnServerButtonClicked);
            clientServerButton.onClick.AddListener(OnClientButtonClicked);

            for (int i = 0; i < BalanceManager.Maps.Count; i++)
            {
                mapSlots.Add(Instantiate(mapSlotPrototype, mapsContentHolder));
                mapSlots[i].EventLobbyMapSlotSelected += OnLobbyMapSlotSelected;
                mapSlots[i].Initialize(BalanceManager.Maps[i]);
                mapSlots[i].SetSelectState(i == 0);
            }

            for (int i = 0; i < SessionDisplayCount; i++)
            {
                sessionSlots.Add(Instantiate(sessionSlotPrototype, sessionsContentHolder));
                sessionSlots[i].EventLobbySessionSlotSelected += OnLobbySessionSlotSelected;
                sessionSlots[i].Initialize();
            }

            photonManager.EventSessionListUpdated += OnPhotonManagerSessionListUpdated;

            mapSlots[0].Select();
        }

        public void Deinitialize()
        {
            photonManager.EventSessionListUpdated -= OnPhotonManagerSessionListUpdated;

            for (int i = 0; i < SessionDisplayCount; i++)
            {
                sessionSlots[i].EventLobbySessionSlotSelected -= OnLobbySessionSlotSelected;
                sessionSlots[i].Deinitialize();
            }

            foreach (var mapSlot in mapSlots)
            {
                mapSlot.EventLobbyMapSlotSelected -= OnLobbyMapSlotSelected;
                mapSlot.Deinitialize();
            }

            startServerButton.onClick.RemoveListener(OnServerButtonClicked);
            clientServerButton.onClick.RemoveListener(OnClientButtonClicked);
        }

        public void Show(bool autoStartClient)
        {
            gameObject.SetActive(true);

            if (autoStartClient)
                OnClientButtonClicked();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetStatusDisconnectDescription(UdpConnectionDisconnectReason reason)
        {
            statusLabel.text = string.Format(LocalizationHelper.LobbyDisconnectedReasonStringFormat, reason);
        }

        private void OnLobbyMapSlotSelected(LobbyMapSlot lobbyMapSlot)
        {
            selectedMapSlot = lobbyMapSlot;
            selectedMapLabel.text = selectedMapSlot.MapDefinition.MapName;

            foreach (var mapSlot in mapSlots)
                mapSlot.SetSelectState(mapSlot == selectedMapSlot);
        }

        private void OnLobbySessionSlotSelected(LobbySessionSlot lobbySessionSlot)
        {
            statusLabel.text = LocalizationHelper.LobbyConnectionStartString;

            SetInputState(false);

            photonManager.StartConnection(lobbySessionSlot.UdpSession, OnConnectSuccess, OnConnectFail);
        }

        private void OnServerButtonClicked()
        {
            statusLabel.text = LocalizationHelper.LobbyServerStartString;

            SetInputState(false);

            photonManager.StartServer(new ServerRoomToken(serverNameInput.text, selectedMapSlot.MapDefinition.MapName), OnServerStartSuccess, OnServerStartFail);
        }

        private void OnClientButtonClicked()
        {
            statusLabel.text = LocalizationHelper.LobbyClientStartString;

            SetInputState(false);

            photonManager.StartClient(OnClientStartSuccess, OnClientStartFail);
        }

        private void OnServerStartFail()
        {
            statusLabel.text = LocalizationHelper.LobbyServerStartFailedString;

            SetInputState(true);
        }

        private void OnServerStartSuccess()
        {
            statusLabel.text = LocalizationHelper.LobbyServerStartSuccessString;

            SetInputState(true);

            lobbyScreen.Hide();
        }

        private void OnClientStartFail()
        {
            statusLabel.text = LocalizationHelper.LobbyClientStartFailedString;

            SetInputState(true);
        }

        private void OnClientStartSuccess()
        {
            statusLabel.text = LocalizationHelper.LobbyClientStartSuccessString;

            SetInputState(true);
        }

        private void OnConnectFail()
        {
            statusLabel.text = LocalizationHelper.LobbyClientConnectFailedString;

            SetInputState(true);
        }

        private void OnConnectSuccess()
        {
            statusLabel.text = LocalizationHelper.LobbyClientConnectSuccessString;

            SetInputState(true);

            lobbyScreen.Hide();
        }

        private void OnPhotonManagerSessionListUpdated()
        {
            if (!gameObject.activeInHierarchy)
                return;

            int currentSlotIndex = 0;
            foreach (var session in photonManager.Sessions)
            {
                if (currentSlotIndex >= SessionDisplayCount)
                {
                    Debug.LogError($"To many sessions to display! Available slots {SessionDisplayCount} Received: {photonManager.Sessions.Count}");
                    break;
                }

                sessionSlots[currentSlotIndex].SetSession(session.Value);
                currentSlotIndex++;
            }

            for (int i = currentSlotIndex; i < sessionSlots.Count; i++)
                sessionSlots[i].SetSession(null);
        }
    }
}
