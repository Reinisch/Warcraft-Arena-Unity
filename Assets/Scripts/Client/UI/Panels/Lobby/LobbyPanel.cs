using System.Collections.Generic;
using Client.UI;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Common;

namespace Client
{
    public class LobbyPanel : UIWindow<LobbyPanelType>
    {
        public struct InitData : IPanelInitData
        {
            private readonly PhotonBoltManager photonManager;
            private readonly LobbyScreen lobbyScreen;

            public InitData(PhotonBoltManager photonManager, LobbyScreen lobbyScreen)
            {
                this.photonManager = photonManager;
                this.lobbyScreen = lobbyScreen;
            }

            public void Process<TPanelType>(UIPanel<TPanelType> panel)
            {
                Assert.IsTrue(panel is LobbyPanel);

                if (panel is LobbyPanel lobbyPanel)
                {
                    lobbyPanel.photonManager = photonManager;
                    lobbyPanel.lobbyScreen = lobbyScreen;
                }
            }
        }

        public struct ShowData : IPanelShowData<LobbyPanelType>
        {
            private bool AutoStartClient { get; }
            private DisconnectReason? DisconnectReason { get; }

            public LobbyPanelType PanelType => LobbyPanelType.LobbyPanel;

            public ShowData(bool autoStartClient, DisconnectReason? disconnectReason = null)
            {
                AutoStartClient = autoStartClient;
                DisconnectReason = disconnectReason;
            }

            public void Process(IPanel panel)
            {
                Assert.IsTrue(panel is LobbyPanel);

                if (panel is LobbyPanel lobbyPanel)
                {
                    if (DisconnectReason.HasValue)
                        lobbyPanel.statusLabel.text = string.Format(LocalizationHelper.LobbyDisconnectedReasonStringFormat, DisconnectReason.Value);

                    if (AutoStartClient)
                        lobbyPanel.OnClientButtonClicked();
                }
            }
        }

        [SerializeField, UsedImplicitly] private Button startServerButton;
        [SerializeField, UsedImplicitly] private Button clientServerButton;
        [SerializeField, UsedImplicitly] private Transform mapsContentHolder;
        [SerializeField, UsedImplicitly] private Transform sessionsContentHolder;
        [SerializeField, UsedImplicitly] private LobbyMapSlot mapSlotPrototype;
        [SerializeField, UsedImplicitly] private LobbySessionSlot sessionSlotPrototype;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI selectedMapLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI serverNameInput;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI statusLabel;

        [SerializeField, UsedImplicitly] private GameObject startClientTooltip;
        [SerializeField, UsedImplicitly] private GameObject noSessionsFoundTooltip;

        private readonly List<LobbyMapSlot> mapSlots = new List<LobbyMapSlot>();
        private readonly List<LobbySessionSlot> sessionSlots = new List<LobbySessionSlot>();
        private const int SessionDisplayCount = 20;

        private LobbyScreen lobbyScreen;
        private LobbyMapSlot selectedMapSlot;
        private PhotonBoltManager photonManager;

        public override LobbyPanelType PanelType => LobbyPanelType.LobbyPanel;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

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

        protected override void PanelDeinitialized()
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

            base.PanelDeinitialized();
        }

        protected override void PanelShown()
        {
            base.PanelShown();

            gameObject.SetActive(true);
            startClientTooltip.SetActive(true);
            noSessionsFoundTooltip.SetActive(false);

            ResetSessions();
        }

        protected override void PanelHidden()
        {
            gameObject.SetActive(false);
            startClientTooltip.SetActive(false);
            noSessionsFoundTooltip.SetActive(false);

            ResetSessions();

            base.PanelHidden();
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

            UpdateInputState(false);

            photonManager.StartConnection(lobbySessionSlot.UdpSession, OnConnectSuccess, OnConnectFail);
        }

        private void OnServerButtonClicked()
        {
            statusLabel.text = LocalizationHelper.LobbyServerStartString;

            UpdateInputState(false);

            photonManager.StartServer(new ServerRoomToken(serverNameInput.text, selectedMapSlot.MapDefinition.MapName), OnServerStartSuccess, OnServerStartFail);
        }

        private void OnClientButtonClicked()
        {
            statusLabel.text = LocalizationHelper.LobbyClientStartString;

            UpdateInputState(false);

            photonManager.StartClient(OnClientStartSuccess, OnClientStartFail);
        }

        private void OnServerStartFail()
        {
            statusLabel.text = LocalizationHelper.LobbyServerStartFailedString;

            UpdateInputState(true);
        }

        private void OnServerStartSuccess()
        {
            statusLabel.text = LocalizationHelper.LobbyServerStartSuccessString;

            UpdateInputState(true);

            lobbyScreen.Hide();
        }

        private void OnClientStartFail()
        {
            statusLabel.text = LocalizationHelper.LobbyClientStartFailedString;
            startClientTooltip.SetActive(true);

            UpdateInputState(true);
        }

        private void OnClientStartSuccess()
        {
            statusLabel.text = LocalizationHelper.LobbyClientStartSuccessString;
            startClientTooltip.SetActive(false);
            noSessionsFoundTooltip.SetActive(photonManager.Sessions.Count == 0);

            UpdateInputState(true);
        }

        private void OnConnectFail()
        {
            statusLabel.text = LocalizationHelper.LobbyClientConnectFailedString;

            UpdateInputState(true);
        }

        private void OnConnectSuccess()
        {
            statusLabel.text = LocalizationHelper.LobbyClientConnectSuccessString;

            UpdateInputState(true);

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

            startClientTooltip.SetActive(false);
            noSessionsFoundTooltip.SetActive(photonManager.Sessions.Count == 0);
        }

        private void ResetSessions()
        {
            foreach (var session in sessionSlots)
                session.SetSession(null);
        }
    }
}
