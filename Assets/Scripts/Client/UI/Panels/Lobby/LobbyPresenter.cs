using System.Collections.Generic;
using Client.UI;
using Common;
using Core;
using Cysharp.Threading.Tasks;
using Net;
using UnityEngine;
using Zenject;

namespace Client
{
    public class LobbyPresenter : Presenter<LobbyPanel>
    {
        private const float ConnectSweepSeconds = 1.25f;

        [Inject] private BalanceReference balance;
        [Inject] private GameSession gameSession;

        private readonly List<LobbyMapSlot> mapSlots = new();
        private readonly List<LobbySessionSlot> sessionSlots = new();

        private LobbyMapSlot selectedMapSlot;
        private float connectProgress;
        private string pendingDisconnectReason;

        public override void Initialize(LobbyPanel view)
        {
            base.Initialize(view);

            view.PlayerNameInput.text = PlayerPrefs.GetString(PrefUtils.PlayerNamePref, $"Player{Random.Range(1, 99999).ToString(),5}");
            view.ServerNameInput.text = PlayerPrefs.GetString(PrefUtils.PlayerServerNamePref, $"\"{view.PlayerNameInput.text}\" Server");

            view.PlayerNameInput.onValueChanged.AddListener(OnPlayerNameChanged);
            view.ServerNameInput.onValueChanged.AddListener(OnServerNameChanged);

            view.StartServerButton.onClick.AddListener(OnServerButtonClicked);
            view.SinglePlayerButton.onClick.AddListener(OnSinglePlayerButtonClicked);
            view.LoadAdditiveButton.onClick.AddListener(OnLoadAdditiveButtonClicked);
            view.StartClientButton.onClick.AddListener(OnClientButtonClicked);
            view.LeaveButton.onClick.AddListener(OnLeaveButtonClicked);
            view.CloseButton.onClick.AddListener(OnCloseButtonClicked);
            view.RegionDropdown.onValueChanged.AddListener(OnRegionDropdownChanged);
            view.LocalSessionsTabButton.onClick.AddListener(OnLocalSessionsTabClicked);
            view.OnlineSessionsTabButton.onClick.AddListener(OnOnlineSessionsTabClicked);
            if (view.RefreshSessionsButton != null)
                view.RefreshSessionsButton.onClick.AddListener(OnRefreshSessionsClicked);

            gameSession.EventStateChanged += RefreshActions;
            gameSession.EventJoinFailed += OnJoinFailed;
            gameSession.EventSessionLost += OnSessionLost;
            gameSession.EventSessionsChanged += RefreshSessions;

            view.RegionDropdown.gameObject.SetActive(false);
            view.StartClientTooltip.SetActive(false);
            view.NoSessionsFoundTooltip.SetActive(false);
            view.VersionName.text = $"v{gameSession.Version}";
            view.StatusLabel.SetEmpty();

            for (int i = 0; i < balance.Scenarios.Count; i++)
            {
                var mapSlot = Object.Instantiate(view.MapSlotPrototype, view.MapsContentHolder);
                mapSlot.EventLobbyMapSlotSelected += OnLobbyMapSlotSelected;
                mapSlot.Initialize(balance.Scenarios[i]);
                mapSlot.SetSelectState(i == 0);

                mapSlots.Add(mapSlot);
            }

            if (mapSlots.Count > 0)
                mapSlots[0].Select();

            RefreshSessions();
            UpdateSessionTabs();
            RefreshActions();
        }

        public override void Deinitialize()
        {
            View.PlayerNameInput.onValueChanged.RemoveListener(OnPlayerNameChanged);
            View.ServerNameInput.onValueChanged.RemoveListener(OnServerNameChanged);

            View.StartServerButton.onClick.RemoveListener(OnServerButtonClicked);
            View.SinglePlayerButton.onClick.RemoveListener(OnSinglePlayerButtonClicked);
            View.LoadAdditiveButton.onClick.RemoveListener(OnLoadAdditiveButtonClicked);
            View.StartClientButton.onClick.RemoveListener(OnClientButtonClicked);
            View.LeaveButton.onClick.RemoveListener(OnLeaveButtonClicked);
            View.CloseButton.onClick.RemoveListener(OnCloseButtonClicked);
            View.RegionDropdown.onValueChanged.RemoveListener(OnRegionDropdownChanged);
            View.LocalSessionsTabButton.onClick.RemoveListener(OnLocalSessionsTabClicked);
            View.OnlineSessionsTabButton.onClick.RemoveListener(OnOnlineSessionsTabClicked);
            if (View.RefreshSessionsButton != null)
                View.RefreshSessionsButton.onClick.RemoveListener(OnRefreshSessionsClicked);

            gameSession.EventStateChanged -= RefreshActions;
            gameSession.EventJoinFailed -= OnJoinFailed;
            gameSession.EventSessionLost -= OnSessionLost;
            gameSession.EventSessionsChanged -= RefreshSessions;

            foreach (var mapSlot in mapSlots)
            {
                mapSlot.EventLobbyMapSlotSelected -= OnLobbyMapSlotSelected;
                mapSlot.Deinitialize();
            }

            mapSlots.Clear();
            ClearSessionSlots();

            base.Deinitialize();
        }

        public void Shown()
        {
            View.StartClientTooltip.SetActive(false);
            View.NoSessionsFoundTooltip.SetActive(false);

            if (pendingDisconnectReason != null)
            {
                View.StatusLabel.SetString(View.DisconnectedReasonString, pendingDisconnectReason);
                pendingDisconnectReason = null;
            }
            else if (!gameSession.IsConnecting)
            {
                View.StatusLabel.SetEmpty();
            }

            gameSession.RefreshSessions();
            RefreshSessions();
            RefreshActions();
        }

        public void Hidden()
        {
            View.StartClientTooltip.SetActive(false);
            View.NoSessionsFoundTooltip.SetActive(false);
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

            View.SelectedMapLabel.text = selectedMapSlot.ScenarioDefiniton.Map.MapName;

            if (!string.IsNullOrEmpty(selectedMapSlot.ScenarioDefiniton.ScenarioName))
                View.SelectedMapLabel.text += $" {selectedMapSlot.ScenarioDefiniton.ScenarioName}";

            foreach (var mapSlot in mapSlots)
                mapSlot.SetSelectState(mapSlot == selectedMapSlot);

            gameSession.SetSessionSource(selectedMapSlot.ScenarioDefiniton.SupportsMultiplayer
                ? SessionSource.UnityServices
                : SessionSource.Lan);

            RefreshActions();
        }

        private void OnRegionDropdownChanged(int index)
        {
        }

        private void RefreshSessions()
        {
            ClearSessionSlots();

            if (View.SessionSlotPrototype == null || View.SessionsContentHolder == null)
                return; // session list not present in this prefab yet

            var sessions = gameSession.AvailableSessions;
            for (int i = 0; i < sessions.Count; i++)
            {
                var slot = Object.Instantiate(View.SessionSlotPrototype, View.SessionsContentHolder);
                slot.EventLobbySessionSlotSelected += OnSessionSlotSelected;
                slot.Initialize(sessions[i]);
                sessionSlots.Add(slot);
            }

            if (View.NoSessionsFoundTooltip != null)
                View.NoSessionsFoundTooltip.SetActive(sessions.Count == 0);
        }

        private void OnLocalSessionsTabClicked()
        {
            gameSession.SetSessionSource(SessionSource.Lan);
            RefreshActions();
        }

        private void OnOnlineSessionsTabClicked()
        {
            gameSession.SetSessionSource(SessionSource.UnityServices);
            RefreshActions();
        }

        private void OnRefreshSessionsClicked()
        {
            gameSession.RefreshSessions();
            View.NoSessionsFoundTooltip.SetActive(false);
        }

        private void UpdateSessionTabs()
        {
            bool canSwitch = gameSession.CanStartNewSession;
            View.LocalSessionsTabButton.interactable = canSwitch && gameSession.SessionSource != SessionSource.Lan;
            View.OnlineSessionsTabButton.interactable = canSwitch && gameSession.SessionSource != SessionSource.UnityServices;
        }

        private void ClearSessionSlots()
        {
            foreach (var slot in sessionSlots)
            {
                slot.EventLobbySessionSlotSelected -= OnSessionSlotSelected;
                slot.Deinitialize();
                Object.Destroy(slot.gameObject);
            }

            sessionSlots.Clear();
        }

        private async void OnSessionSlotSelected(LobbySessionSlot slot)
        {
            if (gameSession.IsConnecting)
                return;

            View.StatusLabel.SetEmpty();
            await gameSession.JoinAsync(slot.Session);
            if (gameSession.State == GameSessionState.Client)
                View.Hide();
        }

        private async void OnServerButtonClicked()
        {
            if (selectedMapSlot == null)
                return;

            View.StatusLabel.SetString(View.ServerStartString);
            await gameSession.StartHostAsync(selectedMapSlot.ScenarioDefiniton);
            if (gameSession.State == GameSessionState.Host)
                View.Hide();          // success → enter the game
            else
                View.StatusLabel.SetString(View.ServerStartFailedString);
        }

        private async void OnSinglePlayerButtonClicked()
        {
            if (selectedMapSlot == null)
                return;

            // Single-player is a local host, so it reuses the server start feedback.
            View.StatusLabel.SetString(View.ServerStartString);
            await gameSession.StartSinglePlayerAsync(selectedMapSlot.ScenarioDefiniton);
            if (gameSession.State == GameSessionState.SinglePlayer)
                View.Hide();
            else
                View.StatusLabel.SetString(View.ServerStartFailedString);
        }

        private async void OnLoadAdditiveButtonClicked()
        {
            if (selectedMapSlot == null)
                return;

            await gameSession.LoadAdditiveAsync(selectedMapSlot.ScenarioDefiniton);
            View.Hide();
        }

        private async void OnClientButtonClicked()
        {
            if (gameSession.IsConnecting)
            {
                gameSession.CancelJoin();
                View.StatusLabel.SetEmpty();
                return;
            }

            View.StatusLabel.SetEmpty();
            await gameSession.JoinAsync(BuildDirectSession());
            if (gameSession.State == GameSessionState.Client)
                View.Hide();
        }

        private SessionInfo BuildDirectSession()
        {
            string raw = View.DirectConnectAddressInput != null ? View.DirectConnectAddressInput.text : null;
            if (string.IsNullOrWhiteSpace(raw))
                raw = "127.0.0.1";

            string host = raw.Trim();
            int port = 0;
            int colon = host.LastIndexOf(':');
            if (colon > 0 && int.TryParse(host[(colon + 1)..], out int parsedPort))
            {
                port = parsedPort;
                host = host[..colon];
            }

            return new SessionInfo(string.Empty, host, string.Empty, string.Empty, 0, 0,
                address: host, port: port, source: SessionSource.Lan);
        }

        private void OnLeaveButtonClicked()
        {
            View.StatusLabel.SetEmpty();
            gameSession.LeaveAsync().Forget();
        }

        private void RefreshActions()
        {
            bool hasMap = selectedMapSlot != null;
            bool connecting = gameSession.IsConnecting;

            // Single-player is a LOCAL host; under the Online (Unity services) host target it would create a
            // public cloud session instead, so disable it there to avoid confusion — it's only a Local-target action.
            bool localHostTarget = gameSession.SessionSource == SessionSource.Lan;
            bool scenarioSupportsMp = hasMap && selectedMapSlot.ScenarioDefiniton.SupportsMultiplayer;

            View.StartServerButton.interactable = gameSession.CanStartNewSession && scenarioSupportsMp;
            View.SinglePlayerButton.interactable = gameSession.CanStartNewSession && hasMap && localHostTarget;
            View.LoadAdditiveButton.interactable = gameSession.CanLoadAdditive && hasMap;
            View.LeaveButton.interactable = gameSession.CanLeave;

            // Join ⇄ Cancel: enabled to start a join (clean state) or to cancel one in progress.
            View.StartClientButton.interactable = gameSession.CanStartNewSession || connecting;
            if (View.StartClientButtonLabel != null)
                View.StartClientButtonLabel.text = connecting ? "Cancel" : "Connect";

            if (View.ConnectingProgressBar != null)
                View.ConnectingProgressBar.gameObject.SetActive(connecting);

            if (connecting)
            {
                connectProgress = 0f;
                View.StatusLabel.SetString(View.ClientStartString);
            }

            UpdateSessionTabs();
        }

        public override void Tick(float deltaTime)
        {
            if (View == null || !gameSession.IsConnecting || View.ConnectingProgressBar == null)
                return;

            connectProgress += deltaTime / ConnectSweepSeconds;
            if (connectProgress > 1f)
                connectProgress -= 1f;

            RectTransform bar = View.ConnectingProgressBar.rectTransform;
            Vector2 max = bar.anchorMax;
            max.x = connectProgress;
            bar.anchorMax = max;
            bar.offsetMin = Vector2.zero;
            bar.offsetMax = Vector2.zero;
        }

        private void OnJoinFailed(string reason)
        {
            if (!string.IsNullOrEmpty(reason))
                View.StatusLabel.SetString(View.DisconnectedReasonString, reason);
            else
                View.StatusLabel.SetString(View.ClientStartFailedString);
        }

        private void OnSessionLost(string reason)
        {
            pendingDisconnectReason = reason;
        }

        private void OnCloseButtonClicked()
        {
            View.Hide();
        }
    }
}
