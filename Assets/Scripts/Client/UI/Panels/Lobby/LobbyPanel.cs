using System.Collections.Generic;
using Client.Localization;
using Client.UI;
using Common;
using Core;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Client
{
    public class LobbyPanel : UIWindow<LobbyScreen>
    {
        public struct RegisterToken : IPanelRegisterToken<LobbyPanel>
        {
            public void Initialize(LobbyPanel panel)
            {
                panel.gameObject.SetActive(false);
            }
        }

        public struct UnregisterToken : IPanelUnregisterToken<LobbyPanel>
        {
            public void Deinitialize(LobbyPanel panel)
            {
                panel.gameObject.SetActive(false);
            }
        }

        public struct ShowToken : IPanelShowToken<LobbyPanel>
        {
            private bool AutoStartClient { get; }
            private DisconnectReason? DisconnectReason { get; }

            public ShowToken(bool autoStartClient, DisconnectReason? disconnectReason = null)
            {
                AutoStartClient = autoStartClient;
                DisconnectReason = disconnectReason;
            }

            public void Process(LobbyPanel panel)
            {
                if (DisconnectReason.HasValue)
                    panel.statusLabel.SetString(panel.disconnectedReasonString, DisconnectReason.Value);
                else
                    panel.statusLabel.SetEmpty();

                if (AutoStartClient)
                    panel.OnClientButtonClicked();
            }
        }

        [SerializeField, UsedImplicitly] private BalanceReference balance;
        [SerializeField, UsedImplicitly] private PhotonBoltReference photonReference;
        [SerializeField, UsedImplicitly] private Button startServerButton;
        [SerializeField, UsedImplicitly] private Button singlePlayerButton;
        [SerializeField, UsedImplicitly] private Button clientServerButton;
        [SerializeField, UsedImplicitly] private Transform mapsContentHolder;
        [SerializeField, UsedImplicitly] private Transform sessionsContentHolder;
        [SerializeField, UsedImplicitly] private LobbyMapSlot mapSlotPrototype;
        [SerializeField, UsedImplicitly] private LobbySessionSlot sessionSlotPrototype;
        [SerializeField, UsedImplicitly] private TMP_InputField playerNameInput;
        [SerializeField, UsedImplicitly] private TMP_InputField serverNameInput;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI selectedMapLabel;
        [SerializeField, UsedImplicitly] private TextMeshProUGUI versionName;
        [SerializeField, UsedImplicitly] private LocalizedTextMeshProUGUI statusLabel;
        [SerializeField, UsedImplicitly] private TMP_Dropdown regionDropdown;
        [SerializeField, UsedImplicitly] private GameObject startClientTooltip;
        [SerializeField, UsedImplicitly] private GameObject noSessionsFoundTooltip;

        [SerializeField, UsedImplicitly] private LocalizedString disconnectedReasonString;
        [SerializeField, UsedImplicitly] private LocalizedString connectionStartString;
        [SerializeField, UsedImplicitly] private LocalizedString connectSuccessString;
        [SerializeField, UsedImplicitly] private LocalizedString clientStartString;
        [SerializeField, UsedImplicitly] private LocalizedString serverStartString;
        [SerializeField, UsedImplicitly] private LocalizedString serverStartFailedString;
        [SerializeField, UsedImplicitly] private LocalizedString serverStartSuccessString;
        [SerializeField, UsedImplicitly] private LocalizedString clientStartFailedString;
        [SerializeField, UsedImplicitly] private LocalizedString clientStartSuccessString;

        private readonly List<LobbyMapSlot> mapSlots = new List<LobbyMapSlot>();
        private readonly List<LobbySessionSlot> sessionSlots = new List<LobbySessionSlot>();
        private const int SessionDisplayCount = 20;

        private LobbyMapSlot selectedMapSlot;

        protected override void PanelInitialized()
        {
            base.PanelInitialized();

            playerNameInput.text = PlayerPrefs.GetString(PrefUtils.PlayerNamePref, $"Player{Random.Range(1, 99999).ToString().PadLeft(5, ' ')}");
            serverNameInput.text = PlayerPrefs.GetString(PrefUtils.PlayerServerNamePref, $"\"{playerNameInput.text}\" Server");

            regionDropdown.ClearOptions();
            regionDropdown.AddOptions(MultiplayerUtils.AvailableRegionDescriptions);
            BoltRuntimeSettings.instance.UpdateBestRegion(MultiplayerUtils.AvailableRegions[0]);
            regionDropdown.value = 0;

            regionDropdown.onValueChanged.AddListener(OnRegionDropdownChanged);
            startServerButton.onClick.AddListener(OnServerButtonClicked);
            singlePlayerButton.onClick.AddListener(OnSinglePlayerButtonClicked);
            clientServerButton.onClick.AddListener(OnClientButtonClicked);
            playerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
            serverNameInput.onValueChanged.AddListener(OnServerNameChanged);

            for (int i = 0; i < balance.Maps.Count; i++)
            {
                mapSlots.Add(Instantiate(mapSlotPrototype, mapsContentHolder));
                mapSlots[i].EventLobbyMapSlotSelected += OnLobbyMapSlotSelected;
                mapSlots[i].Initialize(balance.Maps[i]);
                mapSlots[i].SetSelectState(i == 0);
            }

            for (int i = 0; i < SessionDisplayCount; i++)
            {
                sessionSlots.Add(Instantiate(sessionSlotPrototype, sessionsContentHolder));
                sessionSlots[i].EventLobbySessionSlotSelected += OnLobbySessionSlotSelected;
                sessionSlots[i].Initialize();
            }

            mapSlots[0].Select();
            versionName.text = photonReference.Version;

            EventHandler.RegisterEvent(photonReference.UnderlyingController, GameEvents.SessionListUpdated, OnPhotonControllerSessionListUpdated);
        }

        protected override void PanelDeinitialized()
        {
            EventHandler.UnregisterEvent(photonReference.UnderlyingController, GameEvents.SessionListUpdated, OnPhotonControllerSessionListUpdated);

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
            singlePlayerButton.onClick.RemoveListener(OnSinglePlayerButtonClicked);
            clientServerButton.onClick.RemoveListener(OnClientButtonClicked);
            regionDropdown.onValueChanged.RemoveListener(OnRegionDropdownChanged);
            playerNameInput.onValueChanged.RemoveListener(OnPlayerNameChanged);
            serverNameInput.onValueChanged.AddListener(OnServerNameChanged);

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

        private void StartClient(bool forceRestart)
        {
            photonReference.StartClient(OnClientStartSuccess, OnClientStartFail, forceRestart);

            void OnClientStartFail()
            {
                statusLabel.SetString(clientStartFailedString);
                startClientTooltip.SetActive(true);

                UpdateInputState(true);
            }

            void OnClientStartSuccess()
            {
                statusLabel.SetString(clientStartSuccessString);
                startClientTooltip.SetActive(false);
                noSessionsFoundTooltip.SetActive(photonReference.Sessions.Count == 0);

                UpdateInputState(true);
            }
        }

        private void ResetSessions()
        {
            foreach (var session in sessionSlots)
                session.SetSession(null);
        }

        private void OnPlayerNameChanged(string newName)
        {
            PlayerPrefs.SetString(PrefUtils.PlayerNamePref, newName);
        }

        private void OnServerNameChanged(string newName)
        {
            PlayerPrefs.SetString(PrefUtils.PlayerServerNamePref, newName);
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
            statusLabel.SetString(connectionStartString);

            UpdateInputState(false);

            var clientConnectionToken = new ClientConnectionToken
            {
                PrefferedClass = (ClassType) PlayerPrefs.GetInt(UnitUtils.PreferredClassPrefName, 0),
                Name = playerNameInput.text
            };

            photonReference.StartConnection(lobbySessionSlot.UdpSession, clientConnectionToken, OnConnectSuccess, OnConnectFail);

            void OnConnectFail(ClientConnectFailReason failReason)
            {
                statusLabel.SetString(LocalizationReference.Localize(failReason));

                UpdateInputState(true);
            }

            void OnConnectSuccess()
            {
                statusLabel.SetString(connectSuccessString);

                UpdateInputState(true);

                WindowController.HidePanel<LobbyPanel>();
            }
        }

        private void OnRegionDropdownChanged(int index)
        {
            BoltRuntimeSettings.instance.UpdateBestRegion(MultiplayerUtils.AvailableRegions[index]);

            startClientTooltip.SetActive(true);
            noSessionsFoundTooltip.SetActive(false);
            statusLabel.SetString(clientStartString);

            UpdateInputState(false);

            StartClient(true);
        }
        
        private void OnServerButtonClicked()
        {
            statusLabel.SetString(serverStartString);

            UpdateInputState(false);

            photonReference.StartServer(new ServerRoomToken(serverNameInput.text, playerNameInput.text, selectedMapSlot.MapDefinition.MapName), true, OnServerStartSuccess, OnServerStartFail);

            void OnServerStartFail()
            {
                statusLabel.SetString(serverStartFailedString);

                UpdateInputState(true);
            }

            void OnServerStartSuccess()
            {
                statusLabel.SetString(serverStartSuccessString);

                UpdateInputState(true);

                WindowController.HidePanel<LobbyPanel>();
            }
        }

        private void OnSinglePlayerButtonClicked()
        {
            statusLabel.SetString(serverStartString);

            UpdateInputState(false);

            photonReference.StartSinglePlayer(new ServerRoomToken(serverNameInput.text, playerNameInput.text, selectedMapSlot.MapDefinition.MapName), OnServerStartSuccess, OnServerStartFail);

            void OnServerStartFail()
            {
                statusLabel.SetString(serverStartFailedString);

                UpdateInputState(true);
            }

            void OnServerStartSuccess()
            {
                statusLabel.SetString(serverStartSuccessString);

                UpdateInputState(true);

                WindowController.HidePanel<LobbyPanel>();
            }
        }

        private void OnClientButtonClicked()
        {
            statusLabel.SetString(clientStartString);

            UpdateInputState(false);

            StartClient(false);
        }

        private void OnPhotonControllerSessionListUpdated()
        {
            if (!gameObject.activeInHierarchy)
                return;

            int currentSlotIndex = 0;
            foreach (var session in photonReference.Sessions)
            {
                if (currentSlotIndex >= SessionDisplayCount)
                {
                    Debug.LogError($"To many sessions to display! Available slots {SessionDisplayCount} Received: {photonReference.Sessions.Count}");
                    break;
                }

                sessionSlots[currentSlotIndex].SetSession(session.Value);
                currentSlotIndex++;
            }

            for (int i = currentSlotIndex; i < sessionSlots.Count; i++)
                sessionSlots[i].SetSession(null);

            startClientTooltip.SetActive(false);
            noSessionsFoundTooltip.SetActive(photonReference.Sessions.Count == 0);
        }
    }
}
