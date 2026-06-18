using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Net.Ngo
{
    /// <summary>
    /// UDP-broadcast LAN session discovery, independent of NGO./>.
    /// Receives run on the thread pool; all events are marshalled to the main thread (via UniTask.SwitchToMainThread)
    /// </summary>
    internal sealed class LanSessionDiscovery : IDisposable
    {
        private const int DiscoveryPort = 47777;
        private const int BroadcastIntervalMs = 1000;
        private const double SessionTimeoutSeconds = 4.0;
        private const int Magic = 0x57424631; // "WBF1" — cheap foreign-packet reject
        private const int BeaconFormat = 1;   // beacon JSON schema version; bump only on a BREAKING change

        private readonly List<SessionInfo> sessions = new();
        private readonly Dictionary<string, SessionInfo> byKey = new();
        private readonly Dictionary<string, double> lastSeen = new();

        private UdpClient advertiseClient;
        private UdpClient browseClient;
        private CancellationTokenSource advertiseCts;
        private CancellationTokenSource browseCts;

        public IReadOnlyList<SessionInfo> Sessions => sessions;
        public event Action<IReadOnlyList<SessionInfo>> SessionsUpdated;

        public bool IsAdvertising => advertiseClient != null;
        public bool IsBrowsing => browseClient != null;

        // ---- Advertising (host) ----------------------------------------------------------------------------

        /// <summary>Begin broadcasting a session beacon. <paramref name="beaconFactory"/> is invoked on the main
        /// thread before each send so live fields (player count) stay current. Idempotent.</summary>
        public void StartAdvertising(Func<SessionInfo> beaconFactory)
        {
            if (advertiseClient != null || beaconFactory == null)
                return;

            try
            {
                advertiseClient = new UdpClient { EnableBroadcast = true };
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[LAN discovery] failed to start advertising: {e.Message}");
                advertiseClient = null;
                return;
            }

            advertiseCts = new CancellationTokenSource();
            AdvertiseLoop(beaconFactory, advertiseCts.Token).Forget();
        }

        public void StopAdvertising()
        {
            advertiseCts?.Cancel();
            advertiseCts?.Dispose();
            advertiseCts = null;
            advertiseClient?.Dispose();
            advertiseClient = null;
        }

        private async UniTaskVoid AdvertiseLoop(Func<SessionInfo> beaconFactory, CancellationToken token)
        {
            var broadcast = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);
            while (!token.IsCancellationRequested)
            {
                try
                {
                    byte[] payload = Encode(beaconFactory());
                    advertiseClient.Send(payload, payload.Length, broadcast);
                }
                catch (Exception)
                {
                    // Transient send failure (interface down etc.) — keep trying on the next tick.
                }

                try { await UniTask.Delay(BroadcastIntervalMs, cancellationToken: token); }
                catch (OperationCanceledException) { break; }
            }
        }

        // ---- Browsing (client) -----------------------------------------------------------------------------

        /// <summary>Begin listening for session beacons and populating <see cref="Sessions"/>. Idempotent.</summary>
        public void StartBrowsing()
        {
            if (browseClient != null)
                return;

            try
            {
                browseClient = new UdpClient();
                browseClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                browseClient.Client.Bind(new IPEndPoint(IPAddress.Any, DiscoveryPort));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[LAN discovery] failed to start browsing: {e.Message}");
                browseClient?.Dispose();
                browseClient = null;
                return;
            }

            browseCts = new CancellationTokenSource();
            BrowseLoop(browseCts.Token).Forget();
            ExpiryLoop(browseCts.Token).Forget();
        }

        public void StopBrowsing()
        {
            browseCts?.Cancel();
            browseCts?.Dispose();
            browseCts = null;
            browseClient?.Dispose();
            browseClient = null;

            if (byKey.Count > 0)
            {
                byKey.Clear();
                lastSeen.Clear();
                RebuildAndNotify();
            }
        }

        private async UniTaskVoid BrowseLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                UdpReceiveResult result;
                try
                {
                    result = await browseClient.ReceiveAsync();
                }
                catch (ObjectDisposedException) { break; } // socket closed by StopBrowsing
                catch (Exception) { break; }

                SessionInfo? parsed = TryDecode(result.Buffer, result.RemoteEndPoint);
                if (parsed == null)
                    continue;

                await UniTask.SwitchToMainThread();
                if (token.IsCancellationRequested)
                    break;

                Upsert(parsed.Value);
            }
        }

        private async UniTaskVoid ExpiryLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try { await UniTask.Delay(1000, cancellationToken: token); }
                catch (OperationCanceledException) { break; }

                ExpireStale();
            }
        }

        // ---- Session table (main thread) -------------------------------------------------------------------

        private void Upsert(SessionInfo session)
        {
            string key = session.Address + ":" + session.Port;
            lastSeen[key] = Time.realtimeSinceStartupAsDouble;

            if (byKey.TryGetValue(key, out SessionInfo existing) && SameDisplay(existing, session))
                return; // only a keep-alive — nothing the UI needs to see changed

            byKey[key] = session;
            RebuildAndNotify();
        }

        private void ExpireStale()
        {
            double now = Time.realtimeSinceStartupAsDouble;
            List<string> stale = null;
            foreach (var entry in lastSeen)
                if (now - entry.Value > SessionTimeoutSeconds)
                    (stale ??= new List<string>()).Add(entry.Key);

            if (stale == null)
                return;

            foreach (string key in stale)
            {
                byKey.Remove(key);
                lastSeen.Remove(key);
            }

            RebuildAndNotify();
        }

        private void RebuildAndNotify()
        {
            sessions.Clear();
            foreach (SessionInfo s in byKey.Values)
                sessions.Add(s);

            SessionsUpdated?.Invoke(sessions);
        }

        private static bool SameDisplay(SessionInfo a, SessionInfo b) =>
            a.HostName == b.HostName && a.Map == b.Map && a.Version == b.Version &&
            a.PlayerCount == b.PlayerCount && a.MaxPlayers == b.MaxPlayers && a.TeamSize == b.TeamSize;

        // ---- Wire format: versioned JSON -------------------------------------------------------------------
        
        [Serializable]
        private struct BeaconPayload
        {
            public int magic;
            public int format;
            public string id;
            public string host;
            public string map;
            public string version;
            public int players;
            public int max;
            public int port;
            public int teamSize;
        }

        private static byte[] Encode(SessionInfo s) => Encoding.UTF8.GetBytes(JsonUtility.ToJson(new BeaconPayload
        {
            magic = Magic,
            format = BeaconFormat,
            id = s.Id ?? string.Empty,
            host = s.HostName ?? string.Empty,
            map = s.Map ?? string.Empty,
            version = s.Version ?? string.Empty,
            players = s.PlayerCount,
            max = s.MaxPlayers,
            port = s.Port,
            teamSize = s.TeamSize,
        }));

        private static SessionInfo? TryDecode(byte[] data, IPEndPoint sender)
        {
            try
            {
                BeaconPayload p = JsonUtility.FromJson<BeaconPayload>(Encoding.UTF8.GetString(data));
                if (p.magic != Magic)
                    return null; // foreign / malformed packet

                return new SessionInfo(p.id, p.host, p.map, p.version, p.players, p.max,
                    sender.Address.ToString(), p.port, teamSize: p.teamSize);
            }
            catch (Exception)
            {
                return null; // malformed / foreign packet
            }
        }

        public void Dispose()
        {
            StopAdvertising();
            StopBrowsing();
        }
    }
}
