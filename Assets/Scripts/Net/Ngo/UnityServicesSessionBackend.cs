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
    /// Unity Gaming Services session backend (Lobby + Relay) via the Multiplayer Services Sessions API.
    /// </summary>
    internal sealed class UnityServicesSessionBackend
    {
        private const string MapProperty = "map";
        private const string VersionProperty = "version";
        private const string TeamSizeProperty = "teamSize";
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
            try { _ = session.LeaveAsync(); }
            catch (Exception) { /* best effort, app is quitting */ }
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
                int.TryParse(GetProperty(info, TeamSizeProperty), out int teamSize);

                // Id is what JoinAsync joins by; address/port are unused for Relay-backed sessions.
                sessions.Add(new SessionInfo(info.Id, info.Name, map, version,
                    info.MaxPlayers - info.AvailableSlots, info.MaxPlayers, source: SessionSource.UnityServices,
                    teamSize: teamSize));
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
                options.SessionProperties[TeamSizeProperty] =
                    new SessionProperty((token?.TeamSize ?? 0).ToString(), VisibilityPropertyOptions.Public);

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
