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
        public int TeamSize { get; }
        public string Address { get; }
        public int Port { get; }
        public SessionSource Source { get; }

        public SessionInfo(string id, string hostName, string map, string version, int playerCount, int maxPlayers,
            string address = null, int port = 0, SessionSource source = SessionSource.Lan, int teamSize = 0)
        {
            Id = id;
            HostName = hostName;
            Map = map;
            Version = version;
            PlayerCount = playerCount;
            MaxPlayers = maxPlayers;
            TeamSize = teamSize;
            Address = address;
            Port = port;
            Source = source;
        }
    }
}
