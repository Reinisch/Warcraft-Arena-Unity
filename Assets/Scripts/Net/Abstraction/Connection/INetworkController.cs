using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Net
{
    /// <summary>
    /// Top-level lifecycle entry point for networking. Replaces PhotonBoltController /
    /// IPhotonBoltController. Choosing Host / DedicatedServer / RemoteClient here is what makes
    /// "client-as-server", "server only" and "remote client" the same code path with one role.
    /// </summary>
    public interface INetworkController
    {
        NetworkRole Role { get; }
        bool IsRunning { get; }
        string Version { get; }

        /// <summary>True once the local client has finished connecting to a server.</summary>
        bool IsConnectedClient { get; }

        /// <summary>Round-trip time (ms) to the server for a connected remote client; 0 when not a remote
        /// client (server/host/single-player have no meaningful latency to themselves).</summary>
        int RoundTripTimeMs { get; }

        /// <summary>Estimated transport round trip (ms) with local protocol overhead (send-tick batching)
        /// removed — closer to true wire latency than <see cref="RoundTripTimeMs"/>. 0 when not a remote client.</summary>
        int EstimatedWireLatencyMs { get; }

        /// <summary>Where sessions are listed/hosted/joined from (LAN vs Unity services). Drives the lobby tabs.</summary>
        SessionSource SessionSource { get; }

        /// <summary>Switch the active session source. Restarts browsing on the newly-selected source.</summary>
        void SetSessionSource(SessionSource source);

        IReadOnlyList<SessionInfo> Sessions { get; }

        /// <summary>Force an immediate, clean re-scan of discoverable sessions (the lobby's manual refresh).
        /// Recovers a discovery source whose browse loop/socket died, leaving the list stuck empty.</summary>
        void RefreshSessions();

        /// <summary>Human-readable reason for the most recent disconnect/refusal (e.g. a rejected connection's
        /// approval message), for lobby status display. Empty when none.</summary>
        string LastDisconnectReason { get; }

        /// <summary>Server-side: the connection token a connected peer joined with (identity + preferred class +
        /// version), captured at connection approval. Null if unknown.</summary>
        ClientConnectionToken GetConnectionToken(NetId peer);

        /// <summary>Start as a listen server (server logic + local client). The client-as-server case.</summary>
        UniTask<bool> StartHostAsync(INetSerializable sessionToken, bool advertise = true);

        /// <summary>Start as a headless dedicated server (no local client).</summary>
        UniTask<bool> StartServerAsync(INetSerializable sessionToken, bool advertise = true);

        /// <summary>Start as a client able to discover and join sessions.</summary>
        UniTask<bool> StartClientAsync();

        /// <summary>Join a discovered session as a remote client.</summary>
        UniTask<ConnectResult> ConnectAsync(SessionInfo session, INetSerializable connectToken);

        UniTask ShutdownAsync();

        /// <summary>Server: a client connected. Client: connected to the server (id is the server peer).</summary>
        event Action<NetId> PeerConnected;

        event Action<NetId, DisconnectReason> PeerDisconnected;

        event Action<IReadOnlyList<SessionInfo>> SessionsUpdated;

        /// <summary>Networking has fully stopped (shutdown, or lost connection to the server).</summary>
        event Action Stopped;
    }
}
