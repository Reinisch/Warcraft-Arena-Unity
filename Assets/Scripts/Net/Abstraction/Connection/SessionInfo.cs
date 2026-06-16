namespace Net
{
    /// <summary>
    /// A discoverable game session in the session list. Framework-neutral view of what Bolt
    /// exposed as a UdpSession. Carried by <see cref="INetworkController.Sessions"/>.
    /// </summary>
    public readonly struct SessionInfo
    {
        public string Id { get; }
        public string HostName { get; }
        public string Map { get; }
        public string Version { get; }
        public int PlayerCount { get; }
        public int MaxPlayers { get; }
        // Where to connect to join this session. For LAN discovery, Address is captured from the beacon
        // packet's sender and Port is carried in the beacon payload (the host's game port).
        public string Address { get; }
        public int Port { get; }
        // Which backend this session came from — joining routes to the matching one, and the UI tags the name.
        public SessionSource Source { get; }

        public SessionInfo(string id, string hostName, string map, string version, int playerCount, int maxPlayers,
            string address = null, int port = 0, SessionSource source = SessionSource.Lan)
        {
            Id = id;
            HostName = hostName;
            Map = map;
            Version = version;
            PlayerCount = playerCount;
            MaxPlayers = maxPlayers;
            Address = address;
            Port = port;
            Source = source;
        }
    }
}
