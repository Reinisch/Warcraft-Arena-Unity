using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

#pragma warning disable 67 // events kept for interface completeness; unused in single-player

namespace Net.Local
{
    /// <summary>
    /// Dummy single-player controller. There is no real connection: starting "host" just flips the
    /// running flag. Provided so game code can drive the abstraction's lifecycle uniformly; the real
    /// adapter (FishNet, …) replaces this without touching callers.
    /// </summary>
    public sealed class LocalNetworkController : INetworkController
    {
        private static readonly IReadOnlyList<SessionInfo> NoSessions = new SessionInfo[0];

        public NetworkRole Role { get; private set; } = NetworkRole.Host;
        public bool IsRunning { get; private set; }
        public string Version => "0.0.0-local";
        public bool IsConnectedClient => IsRunning && Role == NetworkRole.RemoteClient;
        public int RoundTripTimeMs => 0; // no real connection in single-player
        public int EstimatedWireLatencyMs => 0;
        public SessionSource SessionSource { get; private set; } = SessionSource.Lan;
        public void SetSessionSource(SessionSource source) => SessionSource = source;
        public IReadOnlyList<SessionInfo> Sessions => NoSessions;
        public void RefreshSessions() { } // no discovery in single-player
        public string LastDisconnectReason => string.Empty;
        public ClientConnectionToken GetConnectionToken(NetId peer) => null; // no remote peers in single-player

        public event Action<NetId> PeerConnected;
        public event Action<NetId, DisconnectReason> PeerDisconnected;
        public event Action<IReadOnlyList<SessionInfo>> SessionsUpdated;
        public event Action Stopped;

        public UniTask<bool> StartHostAsync(INetSerializable sessionToken, bool advertise = true)
        {
            Role = NetworkRole.Host;
            IsRunning = true;
            PeerConnected?.Invoke(NetId.None);
            return UniTask.FromResult(true);
        }

        public UniTask<bool> StartServerAsync(INetSerializable sessionToken, bool advertise = true)
        {
            Role = NetworkRole.DedicatedServer;
            IsRunning = true;
            return UniTask.FromResult(true);
        }

        public UniTask<bool> StartClientAsync()
        {
            Role = NetworkRole.RemoteClient;
            IsRunning = true;
            return UniTask.FromResult(true);
        }

        public UniTask<ConnectResult> ConnectAsync(SessionInfo session, INetSerializable connectToken) =>
            UniTask.FromResult(ConnectResult.Ok);

        public UniTask ShutdownAsync()
        {
            if (IsRunning)
            {
                IsRunning = false;
                Role = NetworkRole.None;
                Stopped?.Invoke();
            }

            return UniTask.CompletedTask;
        }
    }
}
