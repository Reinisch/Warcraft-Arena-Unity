namespace Net
{
    /// <summary>
    /// Where the session list / hosting / joining is sourced from. The lobby's two tabs select between them;
    /// the active source backs <see cref="INetworkController.Sessions"/> and host/join routing.
    /// </summary>
    public enum SessionSource
    {
        /// <summary>Same-LAN hosts discovered via UDP broadcast (no cloud).</summary>
        Lan,

        /// <summary>Unity Gaming Services sessions (Lobby + Relay) — joinable over the internet.</summary>
        UnityServices,
    }
}
