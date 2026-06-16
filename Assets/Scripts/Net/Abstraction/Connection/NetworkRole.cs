namespace Net
{
    /// <summary>
    /// Topology role of the local peer. Replaces the old Bolt-era NetworkingMode.
    /// The three project requirements map directly onto these values.
    /// </summary>
    public enum NetworkRole
    {
        None,

        /// <summary>Client-as-server (listen server): runs server logic AND a local client.</summary>
        Host,

        /// <summary>Server logic only, no local client (headless dedicated build).</summary>
        DedicatedServer,

        /// <summary>Client logic only, connected to a remote server.</summary>
        RemoteClient,
    }
}
