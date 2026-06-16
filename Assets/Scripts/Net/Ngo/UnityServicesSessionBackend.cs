using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace Net.Ngo
{
    /// <summary>
    /// Unity Gaming Services session backend (Lobby + Relay) via the Multiplayer Services Sessions API. Hosting
    /// creates a Relay-backed session; joining connects through Relay; browsing polls the session list. The
    /// Sessions API's default Netcode integration configures the transport (Relay) and starts
    /// <c>NetworkManager.Singleton</c> itself, so the existing replication layer (which hooks NetworkManager
    /// directly) works unchanged — the controller only has to prepare the NetworkManager first.
    ///
    /// Requires the project to be linked to a Unity cloud project (Project Settings → Services) with Lobby +
    /// Relay enabled; failures (no project / offline) are caught and surfaced as an empty list / failed host.
    /// </summary>
    internal sealed class UnityServicesSessionBackend
    {
        private const string MapProperty = "map";
        private const string VersionProperty = "version";
        private const int MaxPlayers = 8;
        private const int BrowsePollSeconds = 5;

        private readonly List<SessionInfo> sessions = new();

        private ISession activeSession;
        private CancellationTokenSource browseCts;

        public IReadOnlyList<SessionInfo> Sessions => sessions;
        public event Action SessionsChanged;

        public bool IsBrowsing => browseCts != null;

        public UnityServicesSessionBackend()
        {
#if UNITY_EDITOR
            // The NRE is Editor-only: exiting play mode force-shuts NetworkManager.Singleton, and if we still hold a
            // Unity session its default Netcode handler trips a NullReferenceException in
            // NetworkManagerSession.OnStopCompleted (the NM was stopped WITHOUT going through ISession.LeaveAsync).
            // Application.quitting fires before the NM's OnApplicationQuit, so leaving here registers the session
            // stop synchronously first, which steers OnStopCompleted onto its safe path.
            Application.quitting += OnEditorQuitting;
#endif
        }

#if UNITY_EDITOR
        private void OnEditorQuitting()
        {
            if (activeSession == null)
                return;

            ISession session = activeSession;
            activeSession = null;
            // Fire-and-forget: LeaveAsync's synchronous prefix registers the session stop before NM is torn down;
            // the async tail needn't finish during exit to prevent the NRE.
            try { _ = session.LeaveAsync(); }
            catch (Exception) { /* already gone / nothing we can do during quit */ }
        }
#endif

        // ---- UGS init / auth -------------------------------------------------------------------------------

        private async UniTask<bool> EnsureSignedInAsync()
        {
            try
            {
                if (UnityServices.State != ServicesInitializationState.Initialized)
                    await UnityServices.InitializeAsync();

                if (!AuthenticationService.Instance.IsSignedIn)
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Unity services] sign-in unavailable: {e.Message}");
                return false;
            }
        }

        // ---- Browsing --------------------------------------------------------------------------------------

        public void StartBrowsing()
        {
            if (browseCts != null)
                return;

            browseCts = new CancellationTokenSource();
            BrowseLoop(browseCts.Token).Forget();
        }

        public void StopBrowsing()
        {
            browseCts?.Cancel();
            browseCts?.Dispose();
            browseCts = null;

            if (sessions.Count > 0)
            {
                sessions.Clear();
                SessionsChanged?.Invoke();
            }
        }

        private async UniTaskVoid BrowseLoop(CancellationToken token)
        {
            if (!await EnsureSignedInAsync())
                return;

            while (!token.IsCancellationRequested)
            {
                try
                {
                    QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(new QuerySessionsOptions());
                    if (token.IsCancellationRequested)
                        break;

                    RebuildFrom(results.Sessions);
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[Unity services] session query failed: {e.Message}");
                }

                try { await UniTask.Delay(TimeSpan.FromSeconds(BrowsePollSeconds), cancellationToken: token); }
                catch (OperationCanceledException) { break; }
            }
        }

        private void RebuildFrom(IList<ISessionInfo> infos)
        {
            sessions.Clear();
            foreach (ISessionInfo info in infos)
            {
                string map = GetProperty(info, MapProperty);
                string version = GetProperty(info, VersionProperty);

                // Id is what JoinAsync joins by; address/port are unused for Relay-backed sessions.
                sessions.Add(new SessionInfo(info.Id, info.Name, map, version,
                    info.MaxPlayers - info.AvailableSlots, info.MaxPlayers, source: SessionSource.UnityServices));
            }

            SessionsChanged?.Invoke();
        }

        private static string GetProperty(ISessionInfo info, string key) =>
            info.Properties != null && info.Properties.TryGetValue(key, out SessionProperty prop) ? prop.Value : string.Empty;

        // ---- Host / join / leave ---------------------------------------------------------------------------

        public async UniTask<bool> HostAsync(ServerRoomToken token)
        {
            if (!await EnsureSignedInAsync())
                return false;

            try
            {
                var options = new SessionOptions
                {
                    Name = string.IsNullOrEmpty(token?.Name) ? "Server" : token.Name,
                    MaxPlayers = MaxPlayers,
                }.WithRelayNetwork();

                options.SessionProperties[MapProperty] =
                    new SessionProperty(token?.Map ?? string.Empty, VisibilityPropertyOptions.Public);
                options.SessionProperties[VersionProperty] =
                    new SessionProperty(token?.Version ?? string.Empty, VisibilityPropertyOptions.Public);

                // The default Netcode handler configures Relay + starts the NGO host before this returns.
                activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Unity services] failed to host session: {e.Message}");
                return false;
            }
        }

        public async UniTask<bool> JoinAsync(SessionInfo session)
        {
            if (string.IsNullOrEmpty(session.Id) || !await EnsureSignedInAsync())
                return false;

            try
            {
                // Resolves once the NGO client has finished connecting through Relay.
                activeSession = await MultiplayerService.Instance.JoinSessionByIdAsync(session.Id);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[Unity services] failed to join session: {e.Message}");
                return false;
            }
        }

        public async UniTask LeaveAsync()
        {
            if (activeSession == null)
                return;

            ISession session = activeSession;
            activeSession = null;
            try { await session.LeaveAsync(); }
            catch (Exception e) { Debug.LogWarning($"[Unity services] leave failed: {e.Message}"); }
        }

        public void Dispose()
        {
#if UNITY_EDITOR
            Application.quitting -= OnEditorQuitting;
#endif
            StopBrowsing();
        }
    }
}
