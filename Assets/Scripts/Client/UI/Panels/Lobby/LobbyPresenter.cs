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
        // Indeterminate connect bar sweep period (seconds) — there's no real % to show, just "working".
        private const float ConnectSweepSeconds = 1.25f;

        [Inject] private BalanceReference balance;
        [Inject] private GameSession gameSession;

        private readonly List<LobbyMapSlot> mapSlots = new();
        private readonly List<LobbySessionSlot> sessionSlots = new();

        private LobbyMapSlot selectedMapSlot;
        private float connectProgress;
        // Set when the connection drops; shown the next time the lobby appears (the HUD graph shows it).
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
            view.VersionName.text = string.Empty;
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

            // Note: selecting the default map above (mapSlots[0].Select()) already set the host source from that
            // scenario's multiplayer support, so we don't force a source here.
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

            // Opening the lobby fresh: show a pending disconnect reason if we just dropped, else clear stale
            // status. Then reflect the current session state.
            if (pendingDisconnectReason != null)
            {
                View.StatusLabel.SetString(View.DisconnectedReasonString, pendingDisconnectReason);
                pendingDisconnectReason = null;
            }
            else if (!gameSession.IsConnecting)
            {
                View.StatusLabel.SetEmpty();
            }

            // Opening the lobby forces a clean re-scan (recovers a discovery source that died while we were in a
            // session); the list then repopulates via EventSessionsChanged. Rebuild now from whatever's cached.
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

            // Auto-pick the host target for the scenario: Online for multiplayer-ready maps, LAN for solo-only
            // ones (LAN keeps Single Player available, since that's a Local-target action, while Create Server
            // stays blocked). RefreshActions reflects the new tab + button states.
            gameSession.SetSessionSource(selectedMapSlot.ScenarioDefiniton.SupportsMultiplayer
                ? SessionSource.UnityServices
                : SessionSource.Lan);

            RefreshActions();
        }

        private void OnRegionDropdownChanged(int index)
        {
        }

        // Rebuild the session list from the discoverable sessions (currently the local-network source, empty
        // until the discovery backend is wired). Mirrors the map-slot pattern: instantiate one slot per session
        // from the disabled prototype. The "no sessions found" tooltip doubles as the empty-state placeholder.
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

        // The list shows both LAN and Unity sessions together (tagged by name). These buttons only choose where
        // a NEW host is created (LAN broadcast vs Unity Lobby); the chosen one is shown "pressed".
        private void OnLocalSessionsTabClicked()
        {
            gameSession.SetSessionSource(SessionSource.Lan);
            RefreshActions(); // refreshes Single Player availability + the tab pressed/locked state
        }

        private void OnOnlineSessionsTabClicked()
        {
            gameSession.SetSessionSource(SessionSource.UnityServices);
            RefreshActions();
        }

        // Manual refresh: kick a clean re-scan of both discovery sources. The list updates via
        // EventSessionsChanged once results arrive; clear the empty-state tooltip in the meantime.
        private void OnRefreshSessionsClicked()
        {
            gameSession.RefreshSessions();
            View.NoSessionsFoundTooltip.SetActive(false);
        }

        private void UpdateSessionTabs()
        {
            // The host-target switch only applies when starting a fresh session; lock it while connecting or in
            // a session (switching it then does nothing useful / has unclear behaviour). The non-selected tab is
            // otherwise enabled, the selected one shown as "pressed" (non-interactable).
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

            // Join the discovered session at its advertised LAN address.
            View.StatusLabel.SetEmpty();
            await gameSession.JoinAsync(slot.Session);
            if (gameSession.State == GameSessionState.Client)
                View.Hide();
        }

        // All session lifecycle goes through GameSession, which knows the current world state and only allows
        // valid transitions (start only from None; leave to get back). The lobby is hidden only once we're
        // actually IN the new session (on success) — a failed start/join keeps it open with feedback.
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

        // Direct Connect: join a typed address (sessions are otherwise joined from the list). Doubles as Cancel
        // while connecting (see RefreshActions for the label swap).
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

        // A direct connection to a typed "ip" or "ip:port" (defaults to localhost / the default port). Always a
        // LAN-style direct connect regardless of the host-target selection.
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

        // Enable each action only when it's valid for the current session state (strict gating): new sessions
        // only from a clean state, additive maps only while we own the world, leave only while in a session.
        // While connecting, everything is locked except the Join button, which becomes Cancel.
        private void RefreshActions()
        {
            bool hasMap = selectedMapSlot != null;
            bool connecting = gameSession.IsConnecting;

            // Single-player is a LOCAL host; under the Online (Unity services) host target it would create a
            // public cloud session instead, so disable it there to avoid confusion — it's only a Local-target action.
            bool localHostTarget = gameSession.SessionSource == SessionSource.Lan;
            // Some scenarios (e.g. the bossfight) aren't multiplayer-ready — they can't be hosted for others, but
            // single-player is still fine.
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

            // Connecting/in-session state also gates the host-target switch — keep it in sync on every refresh.
            UpdateSessionTabs();
        }

        // Animate the indeterminate connect bar while a join is in flight (PanelUpdated drives this each frame).
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

        // Show the server's specific refusal (version mismatch / server full) when it gave one; otherwise the
        // generic "couldn't connect" message.
        private void OnJoinFailed(string reason)
        {
            if (!string.IsNullOrEmpty(reason))
                View.StatusLabel.SetString(View.DisconnectedReasonString, reason);
            else
                View.StatusLabel.SetString(View.ClientStartFailedString);
        }

        // Involuntary disconnect: GameSession tears the world down → the HUD graph shows the lobby. We can't
        // set the status now (the graph's lobby-show would wipe it via Shown), so stash the reason and let
        // Shown() display it once the lobby actually appears.
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
