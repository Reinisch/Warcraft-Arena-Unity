namespace Net
{
    /// <summary>Why a peer disconnected. Ported from the old project (de-Photoned).</summary>
    public enum DisconnectReason
    {
        Unknown,
        Timeout,
        Error,
        Disconnected,
        DisconnectedFromServer,
    }
}
