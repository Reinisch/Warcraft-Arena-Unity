using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using Zenject;

namespace Net.Ngo
{
    /// <summary>
    /// <see cref="INetworkController"/> backed by Netcode for GameObjects' <see cref="NetworkManager"/>.
    /// Host / DedicatedServer / RemoteClient map onto StartHost / StartServer / StartClient.
    /// </summary>
    public sealed class NgoNetworkController : INetworkController, IDisposable
    {
        private const int MaxPlayersPerSession = 8;
        private const int MaxPacketQueueSize = 256;

        private bool subscribed;
        private bool handlersRegistered;
        private bool serverPortCaptured;
        private ushort serverPort = 7777;

        private readonly NgoNetSettings settings;
        private readonly DiContainer container;
        private readonly LanSessionDiscovery discovery = new LanSessionDiscovery();
        private readonly UnityServicesSessionBackend unityBackend = new UnityServicesSessionBackend();
        private readonly List<SessionInfo> mergedSessions = new List<SessionInfo>();

        // Connection tokens captured at connection approval, keyed by client id, so the entity spawner can read a
        // joiner's chosen class. Cleared per-client on disconnect and wholesale when networking stops.
        private readonly Dictionary<ulong, ClientConnectionToken> connectionTokens = new Dictionary<ulong, ClientConnectionToken>();

        // Which backend a NEW host is created on. The session LIST always shows both sources merged; this only
        // selects the host target (the lobby's Local/Online buttons). Joining routes by each session's Source.
        private SessionSource hostSource = SessionSource.Lan;

        public NgoNetworkController(NgoNetSettings settings, DiContainer container)
        {
            this.settings = settings;
            this.container = container;

            // The list is the union of both sources; rebuild + notify whenever either changes.
            discovery.SessionsUpdated += _ => RaiseMergedSessions();
            unityBackend.SessionsChanged += RaiseMergedSessions;

            StartBrowsing(); // session-less at boot → browse both sources for hosts
        }

        public string Version => "1.3.0";
        public IReadOnlyList<SessionInfo> Sessions => mergedSessions;

        public bool IsRunning => Manager != null && Manager.IsListening;
        public bool IsConnectedClient => Manager != null && Manager.IsConnectedClient;

        // Reason the server gave for the last disconnect/refusal (e.g. an approval rejection message). NGO sets
        // this string on the client from the server's ConnectionApprovalResponse.Reason before disconnecting.
        public string LastDisconnectReason => Manager != null ? Manager.DisconnectReason : string.Empty;

        public ClientConnectionToken GetConnectionToken(NetId peer) =>
            connectionTokens.TryGetValue(peer.Value, out ClientConnectionToken token) ? token : null;

        // RTT to the server, only meaningful for a pure remote client (a host/server is its own authority).
        // UTP reports it in ms; ServerClientId is the connection a client measures against (same id NGO's own
        // clock sync uses).
        public int RoundTripTimeMs
        {
            get
            {
                NetworkManager nm = Manager;
                if (nm == null || !nm.IsConnectedClient || nm.IsServer)
                    return 0;
                return nm.NetworkConfig.NetworkTransport is UnityTransport transport
                    ? (int)transport.GetCurrentRtt(NetworkManager.ServerClientId)
                    : 0;
            }
        }

        // NGO batches outgoing messages to its send tick, adding up to one tick of delay per hop; subtracting
        // that local overhead from the raw RTT approximates the real transport round trip (e.g. the Relay hop),
        // which on a busy editor + low tick rate is otherwise buried under the batching/processing overhead.
        public int EstimatedWireLatencyMs
        {
            get
            {
                NetworkManager nm = Manager;
                if (nm == null || !nm.IsConnectedClient || nm.IsServer)
                    return 0;
                uint tickRate = nm.NetworkConfig.TickRate;
                int tickOverheadMs = tickRate > 0 ? (int)(2000 / tickRate) : 0; // ~one tick each direction
                return Mathf.Max(0, RoundTripTimeMs - tickOverheadMs);
            }
        }

        public SessionSource SessionSource => hostSource;

        // The list shows both sources regardless, so this only changes the host target — no browsing change.
        public void SetSessionSource(SessionSource source) => hostSource = source;

        // Manual lobby refresh: cleanly restart BOTH sources. This rebinds the LAN socket and spins up a fresh
        // Unity browse loop (re-running sign-in + an immediate query), recovering from any source whose loop or
        // socket died — the cause of a list that stays empty with no way to recover. No-op while in a session.
        public void RefreshSessions()
        {
            if (IsRunning)
                return;

            StopAllBrowsing();
            StartBrowsing();
        }

        private void RaiseMergedSessions()
        {
            mergedSessions.Clear();
            mergedSessions.AddRange(discovery.Sessions);
            mergedSessions.AddRange(unityBackend.Sessions);
            SessionsUpdated?.Invoke(mergedSessions);
        }

        public NetworkRole Role
        {
            get
            {
                NetworkManager nm = Manager;
                if (nm == null || !nm.IsListening)
                    return NetworkRole.None;
                if (nm.IsHost)
                    return NetworkRole.Host;
                return nm.IsServer ? NetworkRole.DedicatedServer : NetworkRole.RemoteClient;
            }
        }

        public event Action<NetId> PeerConnected;
        public event Action<NetId, DisconnectReason> PeerDisconnected;
        public event Action<IReadOnlyList<SessionInfo>> SessionsUpdated;
        public event Action Stopped;

        private static NetworkManager Manager => NetworkManager.Singleton;

        public async UniTask<bool> StartHostAsync(INetSerializable sessionToken, bool advertise = true)
        {
            StopAllBrowsing(); // we're leaving the lobby; don't list ourselves

            // Online (Unity services) host only applies to an ADVERTISED host. A non-advertised host is the
            // single-player case, which must stay local/offline regardless of the selected host target — so it
            // always falls through to the local StartHost path below (never allocates a public cloud session).
            if (advertise && hostSource == SessionSource.UnityServices)
                // The Sessions API allocates Relay, configures the transport and starts the NGO host itself —
                // we just prepare the NetworkManager (scene mgmt off, prefab handlers, callbacks) first.
                return PrepareNetworkManager() != null && await unityBackend.HostAsync(sessionToken as ServerRoomToken);

            bool ok = Start(nm =>
            {
                ConfigureHostPort(nm, advertise);
                return nm.StartHost();
            });
            if (ok && advertise)
                discovery.StartAdvertising(() => BuildBeacon(sessionToken));
            return ok;
        }

        public UniTask<bool> StartServerAsync(INetSerializable sessionToken, bool advertise = true)
        {
            StopAllBrowsing();
            bool ok = Start(nm =>
            {
                ConfigureHostPort(nm, advertise);
                return nm.StartServer();
            });
            if (ok && advertise)
                discovery.StartAdvertising(() => BuildBeacon(sessionToken));
            return UniTask.FromResult(ok);
        }

        public UniTask<bool> StartClientAsync()
        {
            StopAllBrowsing();
            return UniTask.FromResult(Start(nm =>
            {
                ConfigureClientPort(nm);
                return nm.StartClient();
            }));
        }

        public async UniTask<ConnectResult> ConnectAsync(SessionInfo session, INetSerializable connectToken)
        {
            StopAllBrowsing();

            if (session.Source == SessionSource.UnityServices)
            {
                // Sessions API joins Relay + starts the NGO client (resolves once connected). Set the connection
                // payload first so the host's approval callback receives our version + chosen class.
                NetworkManager nm = PrepareNetworkManager();
                if (nm == null)
                    return ConnectResult.Fail(ClientConnectFailReason.FailedToConnectToServer);
                SetConnectionData(nm, connectToken);
                bool joined = await unityBackend.JoinAsync(session);
                return joined ? ConnectResult.Ok : ConnectResult.Fail(ClientConnectFailReason.FailedToConnectToServer);
            }

            bool ok = Start(nm =>
            {
                ConfigureSessionConnection(nm, session);
                SetConnectionData(nm, connectToken); // version + chosen class for the host's approval check
                return nm.StartClient();
            });
            return ok ? ConnectResult.Ok : ConnectResult.Fail(ClientConnectFailReason.FailedToConnectToServer);
        }

        // Build the beacon for the currently-advertised session. Invoked once per broadcast (main thread) so the
        // live player count stays current; static fields come from the host's session token.
        private SessionInfo BuildBeacon(INetSerializable sessionToken)
        {
            var room = sessionToken as ServerRoomToken;
            string name = string.IsNullOrEmpty(room?.Name) ? "Server" : room.Name;
            string map = room?.Map ?? string.Empty;
            NetworkManager nm = Manager;
            int players = nm != null ? nm.ConnectedClientsIds.Count : 0;
            return new SessionInfo(name, name, map, Version, players, MaxPlayersPerSession,
                address: null, port: serverPort, teamSize: room?.TeamSize ?? 0);
        }

        public async UniTask ShutdownAsync()
        {
            // Leave any active Unity session first (also tears down its NGO start); no-op for a LAN session.
            await unityBackend.LeaveAsync();

            NetworkManager nm = Manager;
            if (nm != null && nm.IsListening)
                nm.Shutdown();
        }

        private bool Start(Func<NetworkManager, bool> start)
        {
            NetworkManager nm = PrepareNetworkManager();
            return nm != null && start(nm);
        }

        // Prepare the NetworkManager for a session WITHOUT starting it — shared by the LAN path (which then
        // calls StartHost/StartClient) and the Unity path (where the Sessions API starts NGO afterwards).
        private NetworkManager PrepareNetworkManager()
        {
            NetworkManager nm = Manager;
            if (nm == null)
                return null;

            // We drive scene loading ourselves; NGO's scene sync would duplicate the server's World/map onto the client.
            nm.NetworkConfig.EnableSceneManagement = false;

            // Gate incoming connections: the host approves/refuses each client by version + player cap (below).
            // Set on every peer (harmless on a client — only the server's callback runs) so the config matches.
            nm.NetworkConfig.ConnectionApproval = true;
            nm.ConnectionApprovalCallback = HandleConnectionApproval;

            RegisterPrefabHandlers(nm);
            Subscribe(nm);
            return nm;
        }

        private void HandleConnectionApproval(NetworkManager.ConnectionApprovalRequest request,
            NetworkManager.ConnectionApprovalResponse response)
        {
            response.CreatePlayerObject = false; // we spawn our own Core-backed shadows, not NGO player objects
            response.Pending = false;

            // The host's own local client sends no token — always approve it.
            if (request.ClientNetworkId == NetworkManager.ServerClientId)
            {
                response.Approved = true;
                return;
            }

            NetworkManager nm = Manager;
            if (nm != null && nm.ConnectedClientsIds.Count >= MaxPlayersPerSession)
            {
                response.Approved = false;
                response.Reason = "Server is full";
                return;
            }

            ClientConnectionToken token = ReadConnectionToken(request.Payload);
            if (token == null || !token.IsValid)
            {
                response.Approved = false;
                response.Reason = "Invalid connection token";
                return;
            }

            if (token.Version != Version)
            {
                response.Approved = false;
                response.Reason = $"Version mismatch (server {Version})";
                return;
            }

            connectionTokens[request.ClientNetworkId] = token;
            response.Approved = true;
        }

        // The connection payload sent to the host's approval callback: a serialized ClientConnectionToken.
        private static void SetConnectionData(NetworkManager nm, INetSerializable connectToken)
        {
            if (connectToken == null)
            {
                nm.NetworkConfig.ConnectionData = Array.Empty<byte>();
                return;
            }

            using var writer = new NgoWriter(128, 1024);
            connectToken.Write(writer);
            nm.NetworkConfig.ConnectionData = writer.Buffer.ToArray();
        }

        private static ClientConnectionToken ReadConnectionToken(byte[] payload)
        {
            if (payload == null || payload.Length == 0)
                return null;

            var reader = new FastBufferReader(payload, Allocator.Temp);
            try
            {
                var token = new ClientConnectionToken();
                token.Read(new NgoReader(reader));
                return token;
            }
            finally
            {
                reader.Dispose();
            }
        }

        // Browse both sources while session-less; never browse while running (a host would list itself).
        private void StartBrowsing()
        {
            if (IsRunning)
                return;

            discovery.StartBrowsing();
            unityBackend.StartBrowsing();
        }

        private void StopAllBrowsing()
        {
            discovery.StopBrowsing();
            unityBackend.StopBrowsing();
        }

        // Single-player is a listen host nobody joins, so it must NOT bind the shared server port — otherwise a
        // second LOCAL instance (e.g. an MPPM clone, which also boots single-player) fails to start with
        // "address already in use". Advertised hosts/dedicated servers use the configured server port;
        // single-player binds an ephemeral (OS-assigned, port 0) one.
        private void ConfigureHostPort(NetworkManager nm, bool advertise)
        {
            if (nm.NetworkConfig.NetworkTransport is not UnityTransport transport)
                return;

            CaptureServerPort(transport);
            transport.MaxPacketQueueSize = MaxPacketQueueSize;
            ushort port = advertise ? serverPort : (ushort)0;
            // An advertised LAN host must accept connections on every interface (0.0.0.0), not just loopback,
            // so a discovered client connecting to the host's LAN address can actually reach it. Single-player
            // isn't joinable, so it keeps the configured (loopback) listen address.
            string listenAddress = advertise ? "0.0.0.0" : transport.ConnectionData.ServerListenAddress;
            transport.SetConnectionData(transport.ConnectionData.Address, port, listenAddress);
        }

        // Clients always connect to the shared server port at the configured address.
        private void ConfigureClientPort(NetworkManager nm)
        {
            if (nm.NetworkConfig.NetworkTransport is not UnityTransport transport)
                return;

            CaptureServerPort(transport);
            transport.MaxPacketQueueSize = MaxPacketQueueSize;
            transport.SetConnectionData(transport.ConnectionData.Address, serverPort, transport.ConnectionData.ServerListenAddress);
        }

        // Join a discovered session: point the transport at the host's advertised address/port (falling back to
        // the configured defaults if the session carries none, e.g. a manual/direct join).
        private void ConfigureSessionConnection(NetworkManager nm, SessionInfo session)
        {
            if (nm.NetworkConfig.NetworkTransport is not UnityTransport transport)
                return;

            CaptureServerPort(transport);
            transport.MaxPacketQueueSize = MaxPacketQueueSize;
            string address = string.IsNullOrEmpty(session.Address) ? transport.ConnectionData.Address : session.Address;
            ushort port = session.Port > 0 ? (ushort)session.Port : serverPort;
            transport.SetConnectionData(address, port, transport.ConnectionData.ServerListenAddress);
        }

        // Remember the port the transport was configured with (the shared server port) the FIRST time, before
        // single-player ever overrides it to an ephemeral one — otherwise a later "Create Server" would reuse 0.
        private void CaptureServerPort(UnityTransport transport)
        {
            if (serverPortCaptured)
                return;

            if (transport.ConnectionData.Port != 0)
                serverPort = transport.ConnectionData.Port;
            serverPortCaptured = true;
        }

        // Make NGO instantiate the shadow prefabs through Zenject (so [Inject] works on clients too).
        private void RegisterPrefabHandlers(NetworkManager nm)
        {
            if (handlersRegistered)
                return;

            AddHandler(nm, settings != null ? settings.PlayerShadowPrefab : null);
            AddHandler(nm, settings != null ? settings.CreatureShadowPrefab : null);
            handlersRegistered = true;
        }

        private void AddHandler(NetworkManager nm, GameObject prefab)
        {
            if (prefab != null)
                nm.PrefabHandler.AddHandler(prefab, new ZenjectNetworkPrefabInstanceHandler(prefab, container));
        }

        private void Subscribe(NetworkManager nm)
        {
            if (subscribed)
                return;

            nm.OnClientConnectedCallback += OnClientConnected;
            nm.OnClientDisconnectCallback += OnClientDisconnected;
            nm.OnServerStopped += OnStopped;
            nm.OnClientStopped += OnStopped;
            subscribed = true;
        }

        private void Unsubscribe(NetworkManager nm)
        {
            if (!subscribed)
                return;

            nm.OnClientConnectedCallback -= OnClientConnected;
            nm.OnClientDisconnectCallback -= OnClientDisconnected;
            nm.OnServerStopped -= OnStopped;
            nm.OnClientStopped -= OnStopped;
            subscribed = false;
        }

        private void OnClientConnected(ulong clientId) => PeerConnected?.Invoke(new NetId(clientId));

        private void OnClientDisconnected(ulong clientId)
        {
            connectionTokens.Remove(clientId);
            PeerDisconnected?.Invoke(new NetId(clientId), DisconnectReason.Disconnected);
        }

        private void OnStopped(bool _)
        {
            NetworkManager nm = Manager;
            if (nm != null)
                Unsubscribe(nm);

            connectionTokens.Clear(); // session over — drop captured join tokens

            // Back to a session-less state: stop advertising (if we were a LAN host) and resume browsing both
            // sources for the lobby.
            discovery.StopAdvertising();
            StartBrowsing();

            Stopped?.Invoke();
        }

        public void Dispose()
        {
            NetworkManager nm = Manager;
            if (nm != null)
                Unsubscribe(nm);

            discovery.Dispose();
            unityBackend.Dispose();
        }
    }
}
