namespace Net
{
    /// <summary>
    /// Authoritative network clock. Replaces direct reads of BoltNetwork.ServerFrame.
    /// Needed client-side to interpret server-frame-stamped messages (e.g. spell cooldowns).
    /// </summary>
    public interface INetworkTime
    {
        /// <summary>The current server simulation frame, as known by this peer.</summary>
        int ServerFrame { get; }
    }
}
